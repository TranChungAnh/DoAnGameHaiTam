using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public bool canMove = true;

    private Rigidbody2D rb;
    private PlayerAnimation playerAnimation;

    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.down;  // Mặc định nhìn xuống

    public Vector2 FacingDirection => facingDirection; // Cho script khác đọc hướng

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            playerAnimation.SetIdleAnimation(facingDirection);
            return;
        }

        // Di chuyển
        Vector2 velocity = moveInput.normalized * moveSpeed;
        rb.velocity = velocity;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            // Update hướng đang nhìn
            facingDirection = moveInput.normalized;
        }

        // Gọi animation thông qua PlayerAnimation
        playerAnimation.UpdateMomentAnimation(facingDirection, isMoving);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0.01f)
        {
            facingDirection = moveInput.normalized;
        }
    }
}
