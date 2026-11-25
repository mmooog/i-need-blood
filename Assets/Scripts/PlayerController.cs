using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement & Jump")]
    public float moveSpeed = 7f;
    public float jumpForce = 13f;
    public float jumpPrepDelay = 0f; // Set to 0 for instant jump response

    [Header("Timers (Feel)")]
    public float coyoteTime = 0.1f;      // Allows jumping just after walking off a ledge
    public float jumpBufferTime = 0.12f; // Remembers Space press just before landing

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f; // Slightly larger to avoid micro-gaps
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private float moveInput;
    private bool isGrounded;
    private bool isPreparingJump;
    private bool facingRight = true;

    private float coyoteTimer;
    private float jumpBufferTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // 1. Coyote Timer: counts down after leaving the ground
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // 2. Jump Buffer Timer: remembers if you pressed Jump slightly too early
        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        // 3. Trigger jump if both timers are valid
        if (jumpBufferTimer > 0f && coyoteTimer > 0f && !isPreparingJump)
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            StartCoroutine(JumpWithPrep());
        }

        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsGrounded", (isGrounded || coyoteTimer > 0f) && !isPreparingJump);
    }

    IEnumerator JumpWithPrep()
    {
        isPreparingJump = true;
        anim.SetTrigger("JumpPrep");

        if (jumpPrepDelay > 0f)
            yield return new WaitForSeconds(jumpPrepDelay);

        isPreparingJump = false;
        isGrounded = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void FixedUpdate()
    {
        // Raised velocity threshold slightly (0.1f) so small slopes don't break ground detection
        if (!isPreparingJump && rb.linearVelocity.y <= 0.1f)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = false;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
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
