using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_moveState : MoveState
{
    private Boss1 enemy;
    public B1_moveState(Entity entity, FiniteStateMachine stateMachine, D_MoveState stateData, string animBoolName, Boss1 enemy) : base(entity, stateMachine, stateData, animBoolName)
    {
        this.enemy = enemy;
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
        if (isDetectingWall )
        {
            enemy.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
