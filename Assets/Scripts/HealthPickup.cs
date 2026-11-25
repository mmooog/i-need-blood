using System.Collections;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Heal Settings")]
    public int healAmount = 25;
    public Color healFlashColor = Color.green;
    public float flashDuration = 0.15f;

    private bool isCollected = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                isCollected = true;

                // Restore HP without going over maxHealth, and update the HUD bar
                playerHealth.currentHealth = Mathf.Clamp(playerHealth.currentHealth + healAmount, 0, playerHealth.maxHealth);
                if (playerHealth.healthBar != null)
                {
                    playerHealth.healthBar.SetHealth(playerHealth.currentHealth);
                }

                // Trigger the green heal flash on the Player and then remove this pickup
                StartCoroutine(HealFlashAndDestroy(other.gameObject));
            }
        }
    }

    IEnumerator HealFlashAndDestroy(GameObject player)
    {
        // Hide the pickup sprite and collider immediately so it looks collected right away
        SpriteRenderer pickupSprite = GetComponent<SpriteRenderer>();
        if (pickupSprite != null) pickupSprite.enabled = false;

        Collider2D pickupCollider = GetComponent<Collider2D>();
        if (pickupCollider != null) pickupCollider.enabled = false;

        // Flash the Player's sprite green
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            Color originalColor = playerSprite.color;
            playerSprite.color = healFlashColor;
            yield return new WaitForSeconds(flashDuration);
            playerSprite.color = originalColor;
        }

        Destroy(gameObject);
    }
}
