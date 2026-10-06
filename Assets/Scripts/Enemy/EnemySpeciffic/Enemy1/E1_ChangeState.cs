using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_ChangeState : ChargeState
{
    public Enemy1 enemy1;

    public E1_ChangeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState changeData, Enemy1 enemy1)
        : base(entity, stateMachine, animBoolName, changeData)
    {
        this.enemy1 = enemy1;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        enemy1.LookAtPlayer();

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 1. Tấn công tầm gần ngay khi chạm Player
        if (performCloseRangeAction)
        {
            stateMachine.ChangeState(enemy1.meleeAttackState);
            return;
        }

        // 2. NẾU PLAYER RA KHỎI TẦM -> LẬP TỨC QUAY VỀ ĐI TUẦN (Không chờ hết ChargeTime)
        if (!isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy1.moveState);
            return;
        }

        // 3. Chạm tường -> Tìm kiếm
        if (isDetectingWall)
        {
            stateMachine.ChangeState(enemy1.lookForPlayerState);
            return;
        }

        // 4. Hết thời gian Charge
        if (isChargeTimeOver)
        {
            if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy1.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy1.moveState); // Hoặc lookForPlayerState
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}