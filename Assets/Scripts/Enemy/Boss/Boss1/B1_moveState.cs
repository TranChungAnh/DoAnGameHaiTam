using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_moveState : MoveState
{
    private Boss1 boss1;
    public B1_moveState(Entity entity, FiniteStateMachine stateMachine, D_MoveState stateData, string animBoolName, Boss1 boss1) : base(entity, stateMachine, stateData, animBoolName)
    {
        this.boss1 = boss1;
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
        entity.LookAtPlayer();
        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(boss1.playerDetectedState);
        }
        if (isDetectingWall)
        {
            boss1.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(boss1.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
