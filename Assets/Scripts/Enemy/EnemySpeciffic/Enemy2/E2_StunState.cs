using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_StunState : StunState
{
    private Enemy2 enemy2;
    public E2_StunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stundata, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, stundata)
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
        entity.isStunned = true;
    }

    public override void Exit()
    {
        base.Exit();
        entity.isStunned = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isStunTimeOver)
        { 
            if (isPlayerInMinArgoRange)
            {
                stateMachine.ChangeState(enemy2.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy2.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
