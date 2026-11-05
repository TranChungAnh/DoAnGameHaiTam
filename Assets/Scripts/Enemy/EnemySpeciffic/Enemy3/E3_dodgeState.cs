using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_dodgeState : DodgeState
{
    private Enemy3 enemy3;
    public E3_dodgeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_dodgeState dodgeData, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, dodgeData)
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
        if (isPlayerInMaxAgroRange && performCloseRangeAction)
        {
            stateMachine.ChangeState(enemy3.meleeAttackState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy3.lookForPlayerState);
        }
        else if (isPlayerInMaxAgroRange && !performCloseRangeAction)
        {
            stateMachine.ChangeState(enemy3.rangeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
