using UnityEngine;

/// <summary>
/// Player Controller dùng để TEST Map 2 V7.
/// - Di chuyển 8 hướng.
/// - Hỗ trợ Rigidbody2D.
/// - Lật Sprite theo hướng.
/// - Hỗ trợ Animator.
/// - Sprint bằng Left Shift.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Map2TestPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runMultiplier = 1.6f;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input.sqrMagnitude > 1f)
            input.Normalize();

        float speed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            speed *= runMultiplier;

        movement = input * speed;

        // Lật nhân vật theo hướng trái/phải
        if (spriteRenderer != null)
        {
            if (input.x > 0.1f)
                spriteRenderer.flipX = false;
            else if (input.x < -0.1f)
                spriteRenderer.flipX = true;
        }

        // Animator
        if (animator != null)
        {
            animator.SetFloat("MoveX", input.x);
            animator.SetFloat("MoveY", input.y);
            animator.SetFloat("Speed", input.sqrMagnitude);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
#endif
}