using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateMomentAnimation(Vector2 facingDirection, bool isMoving)
    {
        // ĐANG TẤN CÔNG -> không cho Movement ghi đè animation
        if (animator.GetBool("attack"))
        {
            return;
        }

        animator.SetBool("move", isMoving);
        animator.SetBool("idle", !isMoving);

        animator.SetFloat("MoveY", facingDirection.y);
        animator.SetFloat("MoveX", facingDirection.x);

        Debug.Log($"[MOVE LOG] isMoving: {isMoving}");
    }

    public void SetIdleAnimation(Vector2 facingDirection)
    {
        // ĐANG TẤN CÔNG -> không cho chuyển sang Idle
        if (animator.GetBool("attack"))
        {
            return;
        }

        animator.SetBool("move", false);
        animator.SetBool("idle", true);

        animator.SetFloat("MoveY", facingDirection.y);
        animator.SetFloat("MoveX", facingDirection.x);

    }

    // ==============================
    // ATTACK
    // ==============================

    public void SetAttackAnimation(bool isAttacking, Vector2 facingDirection)
    {
        animator.SetBool("attack", isAttacking);

        if (isAttacking)
        {
            // ĐANG ATTACK
            animator.SetBool("move", false);
            animator.SetBool("idle", false);

            animator.SetFloat("MoveY", facingDirection.y);
            animator.SetFloat("MoveX", facingDirection.x);

        }
        else
        {
            // ATTACK KẾT THÚC
            animator.SetBool("attack", false);
            animator.SetBool("move", false);
            animator.SetBool("idle", true);

            animator.SetFloat("MoveY", facingDirection.y);
            animator.SetFloat("MoveX", facingDirection.x);

        }
    }

    public void PlayerActionAnimation(string actionName)
    {
        animator.SetTrigger(actionName);

    }
}