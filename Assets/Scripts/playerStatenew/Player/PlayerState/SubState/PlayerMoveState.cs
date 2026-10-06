using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Dochecks()
    {
        base.Dochecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isExitingState) return;

        // Xử lý quay mặt (Flip) nhân vật theo hướng trái/phải
        if (movementInput.x != 0)
        {
            Movement?.CheckIfFlip(Mathf.RoundToInt(movementInput.x));
        }

        // Set vận tốc di chuyển 8 hướng chuẩn hóa (.normalized tránh đi chéo bị nhanh)
        Movement?.SetVelocity(playerData.movementVelocity, movementInput.normalized);

        // Chuyển về IdleState khi người chơi không nhấn phím di chuyển
        if (movementInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}