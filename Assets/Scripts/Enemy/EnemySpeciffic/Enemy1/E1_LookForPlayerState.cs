using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_LookForPlayerState : LookForPlayerState
{
    public Enemy1 enemy1;
    public E1_LookForPlayerState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_lookForPlayer lookForPlayer, Enemy1 enemy1) : base(entity, stateMachine, animBoolName, lookForPlayer)
    {
        this.enemy1 = enemy1;
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
            stateMachine.ChangeState(enemy1.playerDetectedState);
        }
        else if (isAllTurnsTimeDone)
        {
            stateMachine.ChangeState(enemy1.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
