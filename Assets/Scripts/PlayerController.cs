using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool canMove = true;

    private Rigidbody2D rb;
    private PlayerAnimation playerAnimation;

    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.down;

    public Vector2 FacingDirection => facingDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    private void Update()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            playerAnimation.SetIdleAnimation(facingDirection);
            return;
        }

        rb.velocity = moveInput.normalized * moveSpeed;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                facingDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
            }
            else
            {
                facingDirection = new Vector2(0, Mathf.Sign(moveInput.y));
            }
        }

        playerAnimation.UpdateMomentAnimation(facingDirection, isMoving);
    }

}
