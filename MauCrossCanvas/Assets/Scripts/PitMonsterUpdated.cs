using UnityEngine;

public class PitMonsterUpdated : MonoBehaviour
{
    public bool goLeft = true;
    public float moveSpeed = 2.0f;
    public int direction = 1;
    public Vector2 startingScale;

    [Header("Jump")]
    public float jumpRange = 3f;        // how close the player must be
    public float jumpForce = 6f;
    public float jumpCooldown = 1.5f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Transform player;
    private float nextJumpTime = 0f;

    void Start()
    {
        startingScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (goLeft)
        {
            direction = -1;
            transform.localScale = startingScale;
        }
        else
        {
            direction = 1;
            transform.localScale = new Vector2(-startingScale.x, startingScale.y);
        }
        transform.Translate(new Vector2(direction, 0) * moveSpeed * Time.deltaTime);

        TryJump();
    }

    void TryJump()
    {
        if (player == null || Time.time < nextJumpTime) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= jumpRange && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    bool IsGrounded()
    {
        Bounds b = col.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(b.center, b.size, 0f, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyCollider") || other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            goLeft = !goLeft;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnemyCollider") || other.gameObject.CompareTag("Enemy"))
        {
            goLeft = !goLeft;
        }
    }
}