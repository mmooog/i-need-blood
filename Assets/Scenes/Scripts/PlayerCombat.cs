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

    [Header("Attack Lunge")]
    public float lungeDistance = 0.6f;
    public float lungeDuration = 0.1f;
    public float wallStopDistance = 0.4f; // Stops the dash before hitting a wall
    public LayerMask wallLayers;          // Set this to Ground in the Inspector

    [Header("Damage Impact (Flash & Pushback)")]
    public Color damageFlashColor = Color.red;
    public float flashDuration = 0.12f;
    public float pushbackDistance = 0.5f;
    public float pushbackDuration = 0.1f;

    [Header("Input")]
    public KeyCode attackKey = KeyCode.J;

    private Animator anim;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float nextAttackTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
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
        StartCoroutine(LungeForward());

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name + " for " + attackDamage + " damage!");
            enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
        }
    }

    IEnumerator LungeForward()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        float elapsed = 0f;
        float speed = lungeDistance / lungeDuration;

        while (elapsed < lungeDuration)
        {
            float step = Mathf.Abs(speed * Time.deltaTime);

            Vector2 origin = playerCollider != null ? (Vector2)playerCollider.bounds.center : (Vector2)transform.position;
            RaycastHit2D hitWall = Physics2D.Raycast(origin, Vector2.right * direction, step + wallStopDistance, wallLayers);

            if (hitWall.collider != null)
            {
                yield break;
            }

            transform.position += new Vector3(direction * step, 0f, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // Called automatically when the player takes damage
    public void TriggerDamageImpact()
    {
        StopCoroutine(nameof(DamageFlashRoutine));
        StartCoroutine(nameof(DamageFlashRoutine));

        StopCoroutine(nameof(PushbackRoutine));
        StartCoroutine(nameof(PushbackRoutine));
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

    IEnumerator PushbackRoutine()
    {
        // Pushes backward in the opposite direction of where the player is facing
        float knockbackDirection = -Mathf.Sign(transform.localScale.x);
        float elapsed = 0f;
        float speed = pushbackDistance / pushbackDuration;

        while (elapsed < pushbackDuration)
        {
            float step = Mathf.Abs(speed * Time.deltaTime);

            // Checks for walls behind the player so knockback never clips through tiles
            Vector2 origin = playerCollider != null ? (Vector2)playerCollider.bounds.center : (Vector2)transform.position;
            RaycastHit2D hitWall = Physics2D.Raycast(origin, Vector2.right * knockbackDirection, step + wallStopDistance, wallLayers);

            if (hitWall.collider != null)
            {
                yield break;
            }

            transform.position += new Vector3(knockbackDirection * step, 0f, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
