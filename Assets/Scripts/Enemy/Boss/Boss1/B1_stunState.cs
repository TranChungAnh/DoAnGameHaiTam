using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_stunState : StunState
{
    private Boss1  boss1;
    public B1_stunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stundata, Boss1 boss1) : base(entity, stateMachine, animBoolName, stundata)
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
                stateMachine.ChangeState(boss1.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(boss1.lookForPlayerState);
            }
        }
    }


    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
