using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator=GetComponent<Animator>();
    }

    public void UpdateMomentAnimation(Vector2 facingDirection,bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isIdle", !isMoving);
        animator.SetFloat("MoveY", facingDirection.y);
        animator.SetFloat("MoveX", facingDirection.x);

    }
    public void SetIdleAnimation(Vector2 facingDirection)
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isIdle", true);
        animator.SetFloat("MoveY", facingDirection.y);
        animator.SetFloat("MoveX", facingDirection.x);
    }
    public void PlayerActionAnimation(string actionName)
    {
        animator.SetTrigger(actionName);
    }
}
