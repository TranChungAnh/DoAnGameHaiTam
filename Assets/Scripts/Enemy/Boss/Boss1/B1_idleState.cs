using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_idleState : IdleState
{
    private Boss1 boss1;
    public B1_idleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Boss1 boss1) : base(entity, stateMachine, animBoolName, stateData)
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
        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(boss1.playerDetectedState);
        }
        else if (isIdelTimeOver)
        {
            stateMachine.ChangeState(boss1.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
