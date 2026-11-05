using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_lookforPlayerState : LookForPlayerState
{
    private Enemy3 enemy3;
    public E3_lookforPlayerState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_lookForPlayer lookForPlayer, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, lookForPlayer)
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
        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy3.playerDetectedState);
        }
        else if (isAllTurnsTimeDone)
        {
            stateMachine.ChangeState(enemy3.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
