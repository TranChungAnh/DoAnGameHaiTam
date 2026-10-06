using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_DetectedState : PlayerDetectedState
{
    private Enemy1 enemy1;

    public E1_DetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected detectedData, Enemy1 enemy1)
        : base(entity, stateMachine, animBoolName, detectedData)
    {
        this.enemy1 = enemy1;
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

        if (performCloseRangeAction)
        {
            stateMachine.ChangeState(enemy1.meleeAttackState);
            return;
        }

        if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy1.moveState);
            return;
        }

        if (performLongRangeAction || isPlayerInMinAgrorange)
        {
            stateMachine.ChangeState(enemy1.chargeState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}