using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement & Jump")]
    public float moveSpeed = 7f;
    public float jumpForce = 13f;
    public float jumpPrepDelay = 0.08f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private float moveInput;
    private bool isGrounded;
    private bool hasJumped;
    private bool isPreparingJump;
    private bool facingRight = true;
    private float groundBufferTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (isGrounded)
            groundBufferTimer = 0.08f;
        else
            groundBufferTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Jump") && isGrounded && !hasJumped && !isPreparingJump)
        {
            StartCoroutine(JumpWithPrep());
        }

        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        // Uses the tiny buffer so 1-frame tile micro-gaps never trigger Jump while walking
        anim.SetBool("IsGrounded", (isGrounded || groundBufferTimer > 0f) && !isPreparingJump);
    }

    IEnumerator JumpWithPrep()
    {
        isPreparingJump = true;
        hasJumped = true;
        groundBufferTimer = 0f;

        anim.SetTrigger("JumpPrep");
        yield return new WaitForSeconds(jumpPrepDelay);

        isPreparingJump = false;
        isGrounded = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void FixedUpdate()
    {
        bool wasGrounded = isGrounded;

        if (!isPreparingJump && rb.linearVelocity.y <= 0.05f)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && !wasGrounded)
        {
            hasJumped = false;
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
