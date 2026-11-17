using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class PlayerControl : MonoBehaviour
{
    [Header ("Movement Variables")]
    public float moveSpeed = 10.0f;
    public float jumpForce = 50.0f;
    public float jumpCutMultiplier = .5f;
    public float normalGravity;
    public float fallGravity;
    public float jumpGravity;
    public float dashPower;

    [Header (" ")]
    Rigidbody2D rb;
    public float dashTime;

    [Header("Double Jump")]
    public int jumpCount = 0;
    public bool canDoubleJump;
    public int facingDirection = 1;
    [SerializeField]
    private TrailRenderer tr;
    public Collider2D playerCollider;
    [Header("Bools")]
    public bool isTouchingWall;
    public bool isClimbing;
    public bool isWallJumping;
    public bool canDash = true;
    bool isDashing;
    [Header("Climb Settings")]
    public float wallJumpForceX = 11f;
    public float wallJumpForceY = 13f;
    public float wallJumpDuration = 0.18f;
    public Transform wallCheck;
    public float wallRad;
    public float wallTime;
    public float wallSlideSpeed = 2f;

    [Header("Input Actions")]
    public InputActionReference playerMove;
    public InputActionReference playerJump;
    public InputActionReference playerDash;
    public Vector2 directionP;
    public Vector2 startingScale;

    [Header("Ground Check")]
    public bool isGrounded = false;
    public Transform groundCheck;
    public float groundCheckRad;
    public LayerMask groundLayer;

    public Collider2D C;
    void Start()
    {
        //set rigid body
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravity;
        startingScale = transform.localScale;
    }

    private void Update()
    {
        if (isDashing)
        {
            return;
        }

        if (jumpCount < 1)
        {
            canDoubleJump = true;
        }
        else canDoubleJump = false;

        CheckGrounded();
        if (isGrounded)
        {
            canDash = true;
            jumpCount = 0;
        }
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallRad, groundLayer);

        if (isTouchingWall && !isGrounded)
        {
            isClimbing = true;
            jumpCount = 0;
        }
        else isClimbing = false;

        if (!isClimbing) Flip();
        ApplyVariableGravity();
        directionP = playerMove.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // don't steer for a moment after a wall jump
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(directionP.x * moveSpeed, rb.linearVelocityY);
        }
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(dashPower * facingDirection, 0f);
        }

        // wall slide: don't fall faster than wallSlideSpeed
        if (isClimbing && rb.linearVelocityY < -wallSlideSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, -wallSlideSpeed);
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRad, groundLayer);
    }

    void Flip()
    {
        if (directionP.x > 0)
        {
            facingDirection = 1;
        }
        else if (directionP.x < 0)
        {
            facingDirection = -1;
        }
        transform.localScale = new Vector2(startingScale.x * facingDirection, startingScale.y);
    }

    private void OnEnable()
    {
        playerJump.action.performed += PlayerJump;
        playerDash.action.performed += PlayerDash;
    }

    private void OnDisable()
    {
        playerJump.action.performed -= PlayerJump;
        playerDash.action.performed -= PlayerDash;
    }

    void ApplyVariableGravity()
    {
        if (rb.linearVelocity.y < -.1f)
        {
            rb.gravityScale = fallGravity;
        }
        else if (rb.linearVelocity.y > .1f)
        {
            rb.gravityScale = jumpGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (isClimbing && context.ReadValueAsButton())
        {
            StartCoroutine(WallJump());
        }
        else if (isGrounded && context.ReadValueAsButton())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
        }
        else if (canDoubleJump && context.ReadValueAsButton())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
        }
        else
        {
            if (rb.linearVelocity.y > 0) //still in air
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * jumpCutMultiplier);
            }
        }
    }

    IEnumerator WallJump()
    {
        isWallJumping = true;
        rb.linearVelocity = new Vector2(wallJumpForceX * -facingDirection, wallJumpForceY);
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }

    private void PlayerDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        Debug.Log("Dashing");
        playerCollider.enabled = false;
        canDash = false;
        isDashing = true;
        float currentGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        tr.emitting = true;

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = currentGravity;
        playerCollider.enabled = true;
        isDashing = false;
        tr.emitting = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRad);
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawWireSphere(wallCheck.position, wallRad);
    }
}