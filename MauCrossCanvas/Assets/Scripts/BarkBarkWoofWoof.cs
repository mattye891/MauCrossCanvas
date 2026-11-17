using System.Collections;
using UnityEngine;

public class BarkBarkWoofWoof : MonoBehaviour
{
    public bool goLeft = true;
    public float moveSpeed = 2.0f;
    public int direction = 1;
    public Vector2 startingScale;

    [Header("Charge")]
    public float chargeRange = 5f;
    public float chargeSpeed = 7f;
    public float chargeDuration = 1.0f;
    public float chargeCooldown = 2.0f;

    private Transform player;
    private bool isCharging = false;
    private float chargeEndTime = 0f;
    private float nextChargeTime = 0f;
    bool playerInFront;
    public float whatISay;

    void Start()
    {
        startingScale = transform.localScale;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        playerInFront = (player.position.x - transform.position.x) * direction > 0;
        // Set facing direction
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

        TryStartCharge();

        // Stop charging when time runs out
        if (isCharging && Time.time >= chargeEndTime)
        {
            StopCharge();
        }
        //Check if the wolf is still charging but the player has jumped over, this check works
        else if (isCharging && !playerInFront)
        {
            StartCoroutine(WoofWait());

        }

        float speed = isCharging ? chargeSpeed : moveSpeed;
        transform.Translate(new Vector2(direction, 0) * speed * Time.deltaTime);
    }

    void TryStartCharge()
    {
        if (isCharging || player == null || Time.time < nextChargeTime) return;

        float distance = Vector2.Distance(transform.position, player.position);
        

        if (distance <= chargeRange && playerInFront)
        {
            isCharging = true;
            chargeEndTime = Time.time + chargeDuration;
            
        }
    }

    void StopCharge()
    {
        
        isCharging = false;
        nextChargeTime = Time.time + chargeCooldown;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyCollider") || other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            Debug.Log("Trigger");
            goLeft = !goLeft;
            if (isCharging) StopCharge();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnemyCollider") || other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collision");
            goLeft = !goLeft;
            if (isCharging) StopCharge();
        }
    }
    //have the wolf keep charging for a set amount of time before turning around, currently doesn't work properly
    IEnumerator WoofWait()
    {
        Debug.Log("Coroutune");
        yield return new WaitForSeconds(whatISay);
        goLeft = !goLeft;
        StopCharge();
        
    }
}