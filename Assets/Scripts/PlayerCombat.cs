using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.6f;
    public int attackDamage = 25;
    public float attackCooldown = 0.4f;
    public LayerMask enemyLayers;

    [Header("Attack Lunge (Physics-Driven)")]
    public float lungeSpeed = 12f;
    public float lungeDuration = 0.1f;

    [Header("Damage Impact (Flash Only)")]
    public Color damageFlashColor = Color.red;
    public float flashDuration = 0.12f;

    [Header("Input")]
    public KeyCode attackKey = KeyCode.J;

    private Animator anim;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float nextAttackTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(attackKey))
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");

        if (playerController != null)
        {
            float direction = Mathf.Sign(transform.localScale.x);
            playerController.ApplyHorizontalImpulse(direction, lungeSpeed, lungeDuration, true);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name + " for " + attackDamage + " damage!");
            enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
        }
    }

    // Called automatically when the player takes damage (keeps the parameter so PlayerHealth.cs compiles without changes)
    public void TriggerDamageImpact(bool applyPushback = false)
    {
        StopCoroutine(nameof(DamageFlashRoutine));
        StartCoroutine(nameof(DamageFlashRoutine));
    }

    IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
