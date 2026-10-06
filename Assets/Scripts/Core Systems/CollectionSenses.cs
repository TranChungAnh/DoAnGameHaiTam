using UnityEngine;

public class CollectionSenses : CoreComponent
{
    protected Movement Movement
    {
        get => movement ?? core.GetCoreComponent(ref movement);
    }

    private Movement movement;

    #region Properties

    public Transform WallCheckPos
    {
        get => wallCheckPos;
        private set => wallCheckPos = value;
    }

    public float WallDistamce
    {
        get => wallDistamce;
        set => wallDistamce = value;
    }

    public LayerMask WhatIsGround
    {
        get => whatIsWall;
        set => whatIsWall = value;
    }

    #endregion

    [SerializeField] private float wallDistamce = 0.5f;
    [SerializeField] private float wallCheckRadius = 0.4f; // Bán kính hình tròn check tường
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private Transform wallCheckPos;

    #region Wall Checks

    public bool CheckIfToucingWall()
    {
        if (WallCheckPos == null || Movement == null) return false;

        // Dùng FacingDirection thay vì CurrentVelocity để tránh lỗi velocity = 0 khi đứng im
        Vector2 direction = Movement.FacingDirection;
        if (direction == Vector2.zero) direction = Vector2.right;

        Vector2 circleCenter = (Vector2)WallCheckPos.position + direction * wallDistamce;

        Collider2D hit = Physics2D.OverlapCircle(circleCenter, wallCheckRadius, whatIsWall);
        return hit != null;
    }

    public bool CheckIfToucingWallBack()
    {
        if (WallCheckPos == null || Movement == null) return false;

        Vector2 direction = -Movement.CurrentVelocity.normalized;
        if (direction == Vector2.zero) return false;

        Vector2 circleCenter = (Vector2)WallCheckPos.position + direction * wallDistamce;

        Collider2D hit = Physics2D.OverlapCircle(circleCenter, wallCheckRadius, whatIsWall);
        return hit != null;
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (wallCheckPos != null)
        {
            Gizmos.color = Color.blue;

            Vector3 direction = Vector3.right;
            if (Application.isPlaying && movement != null && movement.CurrentVelocity != Vector2.zero)
            {
                direction = movement.CurrentVelocity.normalized;
            }

            Vector3 circleCenter = wallCheckPos.position + direction * wallDistamce;
            Gizmos.DrawWireSphere(circleCenter, wallCheckRadius);
        }
    }
}