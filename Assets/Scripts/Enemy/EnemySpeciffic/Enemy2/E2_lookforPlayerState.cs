using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_lookforPlayerState : LookForPlayerState
{
    private Enemy2 enemy2;
    public E2_lookforPlayerState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_lookForPlayer lookForPlayer, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, lookForPlayer)
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
        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy2.playerDetectedState);
        }
        else if(isAllTurnsTimeDone)
        {
            stateMachine.ChangeState(enemy2.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
