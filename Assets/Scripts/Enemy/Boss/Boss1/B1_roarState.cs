using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_roarState : RoarState
{
    protected Boss1 boss1;
    private float roarStartTime;

    public B1_roarState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RoarState roarData, Boss1 boss1)
        : base(entity, stateMachine, animBoolName, attackPosition, roarData)
    {
        this.boss1 = boss1;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        roarStartTime = Time.time; // Ghi nhận thời điểm bắt đầu hét
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

         bool isRoarTimeOver = Time.time >= roarStartTime + 1.5f; // Đặt thời gian cứng 1.5 giây hoặc lấy theo data nếu có

        if (isAnimationFinish || isRoarTimeOver)
        {
            if (isPlayerInMinArgoRange)
            {
                stateMachine.ChangeState(boss1.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(boss1.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}