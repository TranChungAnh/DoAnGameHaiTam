using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : CoreComponent
{
    public Rigidbody2D rb { get; private set; }
    public bool canSetVelocity;
    public Vector2 CurrentVelocity { get; private set; }

    private Vector2 workspace;

    public int facingDirection { get; private set; }
    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponentInParent<Rigidbody2D>();
        facingDirection = 1;
        FacingDirection = Vector2.down; 
        canSetVelocity = true;
    }

    public override void LogicUpdate()
    {
        CurrentVelocity = rb.velocity;
    }

    public void SetVelovityZero()
    {
        workspace = Vector2.zero;
        SetFinalVelocity();
    }

    public void SetVelocity(Vector2 velocity)
    {
        workspace = velocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            FacingDirection = Get4Direction(velocity);

            if (velocity.x != 0)
            {
                facingDirection = velocity.x > 0 ? 1 : -1;
            }
        }

        SetFinalVelocity();
    }

    private Vector2 Get4Direction(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.01f) return FacingDirection;

        // So sánh độ lớn X và Y để ép về 4 hướng chuẩn (Trái, Phải, Lên, Down)
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return dir.y > 0 ? Vector2.up : Vector2.down;
        }
    }
    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();

        workspace.Set(
            angle.x * velocity * direction,
            angle.y * velocity
        );

        // Giữ nguyên FacingDirection nếu đứng yên
        if (workspace.sqrMagnitude > 0.01f)
        {
            FacingDirection = Get4Direction(workspace);
        }

        if (workspace.x != 0)
        {
            facingDirection = workspace.x > 0 ? 1 : -1;
        }

        SetFinalVelocity();
    }

    public void SetVelocity(float velocity, Vector2 direction)
    {
        Vector2 moveDir = direction.sqrMagnitude > 0.01f ? direction.normalized : FacingDirection;
        workspace = moveDir * velocity;

        if (direction.sqrMagnitude > 0.01f)
        {
            FacingDirection = Get4Direction(direction);

            if (direction.x != 0)
            {
                facingDirection = direction.x > 0 ? 1 : -1;
            }
        }

        SetFinalVelocity();
    }

    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);

        if (Mathf.Abs(velocity) > 0.01f)
        {
            facingDirection = velocity > 0 ? 1 : -1;
            FacingDirection = Get4Direction(workspace);
        }

        SetFinalVelocity();
    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);

        if (Mathf.Abs(velocity) > 0.01f)
        {
            FacingDirection = Get4Direction(workspace);
        }

        SetFinalVelocity();
    }

    private void SetFinalVelocity()
    {
        if (canSetVelocity)
        {
            rb.velocity = workspace;
            CurrentVelocity = workspace;
        }
    }

    // Hàm Flip đảo chiều tự động (Dành cho Enemy / LookForPlayerState)
    public void Flip()
    {
        facingDirection *= -1;
        FacingDirection = new Vector2(facingDirection, FacingDirection.y);

      
    }

    // Hàm Flip theo Input x (Dành cho Player)
    public void Flip(int xInput)
    {
        facingDirection = xInput;
        FacingDirection = new Vector2(facingDirection, FacingDirection.y);
    }

    public void CheckIfFlip(int xInput)
    {
        if (xInput != 0 && xInput != facingDirection)
        {
            Flip(xInput);
        }
    }

}