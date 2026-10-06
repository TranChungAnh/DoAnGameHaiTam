using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_dodgeState : DodgeState
{
    private Enemy2 enemy2;
    public E2_dodgeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_dodgeState dodgeData, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, dodgeData)
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
        if(isPlayerInMaxAgroRange && performCloseRangeAction)
        {
            stateMachine.ChangeState(enemy2.meleeAttackState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy2.lookForPlayerState);
        }
        else if(isPlayerInMaxAgroRange && !performCloseRangeAction)
        {
            stateMachine.ChangeState(enemy2.rangeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
