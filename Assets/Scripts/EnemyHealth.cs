using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;       // Set different HP per enemy variant in the Inspector
    public float destroyDelay = 1.5f; // Gives Enemy_Death time to finish before disappearing

    [Header("Hit Flash Impact")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private int currentHealth;
    private bool isDead = false;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! HP left: " + currentHealth);

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator HitFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " died!");

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // Stop enemy movement and attacking immediately on death
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        // Freeze physics and disable collider so the corpse cannot hurt the player
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}
