using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectTitle : MonoBehaviour
{
    private float speed;
    private float travelDistance;
    private float xStartPos;

    [SerializeField]
    private float gravity;
    [SerializeField]
    private float damageRadius;

    private Rigidbody2D rb;
    private bool isGravityOn;
    private bool isScheduledToDestroy = false;
    public bool isFireball = false;

    [SerializeField]
    private LayerMask whatIsPlayer;
    [SerializeField]
    private Transform damagePosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0.0f;

        isGravityOn = false;

        xStartPos = transform.position.x;

        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        if (isGravityOn)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void FixedUpdate()
    {
        if (isScheduledToDestroy) return;

        Collider2D[] damageHit = Physics2D.OverlapCircleAll(damagePosition.position, damageRadius, whatIsPlayer);
        if (damageHit.Length > 0)
        {
            foreach (Collider2D collider in damageHit)
            {
                IDamageable damageable = collider.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    damageable.Damage(10f);
                    Debug.Log("Damage dealt to: " + collider.name);

                    if (States.Instance != null)
                    {
                        float currentHealth = States.Instance.currentHealth;
                        if (currentHealth > 0)
                        {
                            Debug.Log("Current Health: 2222222 " + currentHealth);
                            States.Instance.setHealth(currentHealth - 10f);
                        }
                    }
                    else
                    {
                        Debug.LogError("States.Instance is null.");
                    }

                    StartCoroutine(DelayedDestroy(0.1f));
                    isScheduledToDestroy = true;
                    break;
                }
            }
        }

        if (Mathf.Abs(xStartPos - transform.position.x) >= travelDistance && !isGravityOn && !isFireball)
        {
            isGravityOn = true;
            rb.gravityScale = gravity;
        }
    }

    // Nhận thêm Vector2 direction để bắn thẳng về phía Player
    public void FireProjectile(float speed, float travelDistance, float damage, Vector2 direction)
    {
        this.speed = speed;
        this.travelDistance = travelDistance;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = direction.normalized * speed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isScheduledToDestroy) return;

        if (collision.gameObject.CompareTag("Grounded"))
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            rb.gravityScale = 0f;

            transform.rotation = Quaternion.identity;

            StartCoroutine(DelayedDestroy(0.2f));
            isScheduledToDestroy = true;
        }
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (damagePosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(damagePosition.position, damageRadius);
        }
    }
}