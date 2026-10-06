using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputHandler;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 movementInput;
    protected bool dashInput;

    protected Movement Movement => movement != null ? movement : core.GetCoreComponent(ref movement);
    private Movement movement;

    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
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
        player.dashState?.ResetCanDash();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Đọc giá trị đầu vào từ PlayerInputHandler mới
        movementInput = player.inputHandler.rawMovementInput;
        dashInput = player.inputHandler.dashInput;

        // Chuyển sang Primary Attack State
        if (player.inputHandler.attackInput[(int)CombatInputs.primary])
        {
            stateMachine.ChangeState(player.primaryAttackState);
            Debug.Log("<color=green>[STATE] Kich hoat Primary Attack (Chuot Trai!)</color>");
        }
        // Chuyển sang Secondary Attack State
        else if (player.inputHandler.attackInput[(int)CombatInputs.secondary])
        {
            stateMachine.ChangeState(player.secondaryAttackState);
            Debug.Log("<color=yellow>[STATE] Kich hoat Secondary Attack (Chuot Phai!)</color>");
        }
        // Chuyển sang Dash State
        else if (dashInput && player.dashState != null && player.dashState.CheckIfCanDash())
        {
            stateMachine.ChangeState(player.dashState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}