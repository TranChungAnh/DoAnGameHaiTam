using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_StunState : StunState
{
    private Enemy3 enemy3;
    public E3_StunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stundata, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, stundata)
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
                stateMachine.ChangeState(enemy3.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy3.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
