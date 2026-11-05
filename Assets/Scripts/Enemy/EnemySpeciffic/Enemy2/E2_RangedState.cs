using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_RangedState : RangedState
{
    protected Enemy2 enemy2;

    public E2_RangedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangeAttackState rangeData, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, attackPosition, rangeData)
    {
        this.enemy2 = enemy2;
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
                stateMachine.ChangeState(enemy2.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy2.lookForPlayerState);
            }
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
