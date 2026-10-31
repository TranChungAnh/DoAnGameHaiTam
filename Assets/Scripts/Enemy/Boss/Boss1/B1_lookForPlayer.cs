using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_lookForPlayer : LookForPlayerState
{
    public Boss1 boss1;
    public B1_lookForPlayer(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_lookForPlayer lookForPlayer, Boss1 boss1) : base(entity, stateMachine, animBoolName, lookForPlayer)
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
        else if (isAllTurnsTimeDone)
        {
            stateMachine.ChangeState(boss1.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
