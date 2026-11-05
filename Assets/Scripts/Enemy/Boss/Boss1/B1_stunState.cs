using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_stunState : StunState
{
    private Boss1  enemy;
    public B1_stunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stundata, Boss1  enemy) : base(entity, stateMachine, animBoolName, stundata)
    {
        this.enemy = enemy;
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
                stateMachine.ChangeState(enemy.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
