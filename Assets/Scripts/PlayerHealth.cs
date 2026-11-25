using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public GameOverManager gameOverManager;

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        // Press K in Play Mode to test taking 25 damage
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(25);
        }
    }

    public void TakeDamage(int damage, bool applyPushback = true)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        GetComponent<PlayerCombat>()?.TriggerDamageImpact(applyPushback);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }



    void Die()
    {
        isDead = true;
        anim.SetTrigger("Die");

        if (GetComponent<PlayerController>() != null)
            GetComponent<PlayerController>().enabled = false;
        if (GetComponent<PlayerCombat>() != null)
            GetComponent<PlayerCombat>().enabled = false;

        if (gameOverManager != null)
            gameOverManager.TriggerGameOver();
    }

}
