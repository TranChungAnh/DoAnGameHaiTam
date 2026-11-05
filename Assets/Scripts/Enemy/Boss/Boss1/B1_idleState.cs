using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_idleState : IdleState
{
    private Boss1 enemy;
    public B1_idleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Boss1 enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
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
        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isIdelTimeOver)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
