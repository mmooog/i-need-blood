using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Smooth Movement")]
    public float moveSpeed = 7.5f;
    public float acceleration = 65f;      // How fast she reaches top speed
    public float deceleration = 75f;      // How fast she stops when you let go

    [Header("Crisp Jump & Fall Feel")]
    public float jumpForce = 14f;
    public float fallMultiplier = 2.6f;   // Makes falling faster & snappier than rising
    public float lowJumpMultiplier = 2.2f;// Tapping Spacebar gives a crisp short hop
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.15f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float jumpCooldownTimer;

    // Tracks active dashes/knockbacks so normal walking doesn't cancel them out
    private bool isOverridenByImpulse = false;
    private bool isAttackLocked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Guarantees smooth visuals
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        // 1. Coyote & Jump Buffer Timers
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        // 2. Instant, zero-delay Jump
        if (jumpBufferTimer > 0f && coyoteTimer > 0f && jumpCooldownTimer <= 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            isGrounded = false;
            jumpCooldownTimer = 0.15f;
        }

        // 3. Flip direction (locked briefly during an attack swing so you don't moonwalk mid-lunge)
        if (!isAttackLocked)
        {
            if (moveInput > 0 && !facingRight)
                Flip();
            else if (moveInput < 0 && facingRight)
                Flip();
        }

        // 4. Update Animator cleanly
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
            anim.SetBool("IsGrounded", isGrounded || coyoteTimer > 0f);
        }
    }

    void FixedUpdate()
    {
        // Reliable ground check without velocity-flicker bugs
        if (jumpCooldownTimer <= 0f)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // Smooth acceleration & deceleration when not mid-dash or mid-knockback
        if (!isOverridenByImpulse)
        {
            float targetSpeed = moveInput * moveSpeed;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }

        // Snappy arcade gravity: faster falling + variable jump height when releasing Spacebar early
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    // Called smoothly by PlayerCombat for attack lunges and enemy knockback (uses real physics, zero wall clipping!)
    public void ApplyHorizontalImpulse(float direction, float speed, float duration, bool lockFlip = false)
    {
        StopCoroutine(nameof(ImpulseRoutine));
        StartCoroutine(ImpulseRoutine(direction, speed, duration, lockFlip));
    }

    IEnumerator ImpulseRoutine(float direction, float speed, float duration, bool lockFlip)
    {
        isOverridenByImpulse = true;
        isAttackLocked = lockFlip;

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        yield return new WaitForSeconds(duration);

        isOverridenByImpulse = false;
        isAttackLocked = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
