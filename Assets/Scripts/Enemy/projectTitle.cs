using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectTitle : MonoBehaviour
{
    private float speed;
    private float travelDistance;
    private float xStartPos;

    [SerializeField] private float gravity;
    [SerializeField] private float damageRadius;

    private Rigidbody2D rb;
    private bool isGravityOn;
    private bool isScheduledToDestroy = false;
    public bool isFireball = false;

    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform damagePosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
        isGravityOn = false;
        xStartPos = transform.position.x;
        Destroy(gameObject, 2f);
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

                    if (States.Instance != null)
                    {
                        float currentHealth = States.Instance.currentHealth;
                        if (currentHealth > 0)
                        {
                            States.Instance.setHealth(currentHealth - 10f);
                        }
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

    // 1. Hàm bắn đơn (Dùng cho Enemy thường hoặc Boss bắn thẳng hướng Player)
    public void FireProjectile(float speed, float travelDistance, float damage, Vector2 direction)
    {
        this.speed = speed;
        this.travelDistance = travelDistance;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = direction.normalized * speed;
    }

    // 2. Hàm tĩnh hỗ trợ bắn nhiều quả đạn tỏa ra nhiều hướng (Dành riêng cho Boss)
    public static void FireMultiple(GameObject projectilePrefab, Transform firePoint, float speed, float travelDistance, float damage, int projectileCount)
    {
        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * angleStep;
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            Quaternion shootRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject obj = Instantiate(projectilePrefab, firePoint.position, shootRotation);

            projectTitle script = obj.GetComponent<projectTitle>();
            if (script != null)
            {
                script.FireProjectile(speed, travelDistance, damage, dir);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isScheduledToDestroy) return;

        int playerLayer = LayerMask.NameToLayer("Player");
        int groundLayer = LayerMask.NameToLayer("Ground");

        // Chạm Player: Không Destroy
        if (collision.gameObject.layer == playerLayer)
            return;

        // Chạm Ground: Giữ nguyên xử lý
        if (collision.gameObject.layer == groundLayer)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            rb.gravityScale = 0f;
            transform.rotation = Quaternion.identity;

            StartCoroutine(DelayedDestroy(0.2f));
            isScheduledToDestroy = true;
            return;
        }

        // Chạm các Layer khác: Destroy ngay lập tức
        isScheduledToDestroy = true;
        Destroy(gameObject);
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