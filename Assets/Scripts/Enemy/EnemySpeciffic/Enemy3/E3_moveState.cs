using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_moveState : MoveState
{
    private Enemy3 enemy3;
    public E3_moveState(Entity entity, FiniteStateMachine stateMachine, D_MoveState stateData, string animBoolName, Enemy3 enemy3) : base(entity, stateMachine, stateData, animBoolName)
    {
        this.enemy3 = enemy3;
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
            stateMachine.ChangeState(enemy3.playerDetectedState);
        }
        if (isDetectingWall)
        {
            enemy3.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy3.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
