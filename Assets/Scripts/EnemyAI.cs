using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Variant Stats (Customise Per Enemy!)")]
    public float moveSpeed = 3f;
    public int attackDamage = 15;       // Set different damage for each of your 3 variants
    public float attackCooldown = 1.2f; // Seconds between hits

    [Header("Detection & Attack Ranges")]
    public float chaseRange = 7f;       // Distance at which enemy spots the player and starts walking
    public float attackRange = 1.2f;    // Distance at which enemy stops walking (Idle stance) and attacks

    private Transform player;
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;
    private Animator anim;
    private float nextAttackTime = 0f;
    private bool facingRight = true;
    private float targetSpeedX = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. Player is far away: stay in Enemy_Idle
        if (distanceToPlayer > chaseRange)
        {
            StopMoving();
            return;
        }

        // Face toward the player whenever inside chaseRange (with a small deadzone to prevent flip-jitter when passing through)
        FacePlayer();

        // 2. Player is within chaseRange, but outside attackRange: walk toward player
        if (distanceToPlayer > attackRange)
        {
            MoveTowardPlayer();
        }
        // 3. Player is right in front of enemy: stop in Idle stance and deal damage on cooldown
        else
        {
            StopMoving();

            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void FixedUpdate()
    {
        // Applies movement on the physics step so movement stays smooth
        rb.linearVelocity = new Vector2(targetSpeedX, rb.linearVelocity.y);
    }

    void MoveTowardPlayer()
    {
        if (anim != null) anim.SetBool("IsWalking", true);

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        targetSpeedX = direction * moveSpeed;
    }

    void StopMoving()
    {
        if (anim != null) anim.SetBool("IsWalking", false);
        targetSpeedX = 0f;
    }

    void AttackPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void FacePlayer()
    {
        float direction = player.position.x - transform.position.x;

        // Deadzone of 0.25f stops rapid left/right sprite flickering when your player walks right through the enemy
        if (Mathf.Abs(direction) <= 0.25f) return;

        if (direction > 0 && !facingRight)
            Flip();
        else if (direction < 0 && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Draws visual circles in the Scene view so you can easily resize Chase & Attack ranges!
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
