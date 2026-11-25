using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Deals full maxHealth damage so the HP bar drops to 0 and triggers Die() + GameOver
                playerHealth.TakeDamage(playerHealth.maxHealth);
            }
        }
    }
}
