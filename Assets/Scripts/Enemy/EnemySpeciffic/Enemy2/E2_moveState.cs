using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_moveState : MoveState
{
    private Enemy2 enemy2;
    public E2_moveState(Entity entity, FiniteStateMachine stateMachine, D_MoveState stateData, string animBoolName, Enemy2 enemy2) : base(entity, stateMachine, stateData, animBoolName)
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
        entity.LookAtPlayer();
        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy2.playerDetectedState);
        }
        if( isDetectingWall)
        {
            enemy2.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy2.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
