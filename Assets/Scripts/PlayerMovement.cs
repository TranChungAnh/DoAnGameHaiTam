using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(x, y).normalized;
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition =
            rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }
}