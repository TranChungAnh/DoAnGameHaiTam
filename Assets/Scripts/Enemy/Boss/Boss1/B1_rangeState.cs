using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_rangeState : RangedState 
{
    protected Boss1  boss1;

    public B1_rangeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangeAttackState rangeData, Boss1 boss1) : base(entity, stateMachine, animBoolName, attackPosition, rangeData)
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
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinish)
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
