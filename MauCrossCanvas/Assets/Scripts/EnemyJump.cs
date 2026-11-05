using System.Collections;
using UnityEngine;

public class EnemyJump : MonoBehaviour
{
    Rigidbody2D rb;
    public float jumpForce = 50.0f;
    public float forwardForce;
    Animator animator;
    public GameObject player;
    public bool facingLeft;
    public float attackDistance;
    public float attackTime;
    public bool isAttacking;
    public PitMonster PM;
    public bool isGrounded;
    //public EnemyDeath EM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        PM = GetComponent<PitMonster>();
        //EM = GetComponent<EnemyDeath>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {

            return;
        }

        if (transform.localScale.x > 0)
        {
            facingLeft = true;
        }
        else if (transform.localScale.x < 0)
        {
            facingLeft = false;
        }
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        float directionX = playerPosition.x - transform.position.x;

        bool isPlayerInFront = (facingLeft && directionX > 0) || (!facingLeft && directionX < 0);

        float distance = Vector2.Distance(myPosition, playerPosition);

        // Check if the player is close enough, enemy is facing them and can attack
        if (isGrounded && distance < attackDistance && isPlayerInFront)
        {

            StartCoroutine(JumpAttack());

        }


    }

    private IEnumerator JumpAttack()
    {
        //animator.SetTrigger("jumpAttack");

        isAttacking = true;

        //Vector2 forwardMove = Vector2.Lerp(transform.position, player.transform.position, .5f);
        //Vector2 jumpVelocity = new Vector2((forwardMove.x * PM.direction) , (jumpForce));


        rb.AddForce(Vector2.right * PM.direction * forwardForce, ForceMode2D.Impulse);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);


        yield return new WaitForSeconds(attackTime);
        isAttacking = false;


    }

    private void OnCollisionStay2D(Collision2D ground)
    {
        if (ground.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            //animator.SetBool("canWalk", true);
            //if (!EM.gotKilled)
                PM.enabled = true;

        }
    }

    private void OnCollisionExit2D(Collision2D ground)
    {
        if (ground.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            //animator.SetBool("canWalk", false);
            PM.enabled = false;
        }
    }

}
