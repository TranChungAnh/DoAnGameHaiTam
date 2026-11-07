using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_dodgeState : DodgeState
{
    private Boss1 boss1;
    public B1_dodgeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_dodgeState dodgeData, Boss1 boss1) : base(entity, stateMachine, animBoolName, dodgeData)
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
        if (isPlayerInMaxAgroRange && performCloseRangeAction)
        {
            stateMachine.ChangeState(boss1.meleeAttackState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(boss1.lookForPlayerState);
        }
        else if (isPlayerInMaxAgroRange && !performCloseRangeAction)
        {
            stateMachine.ChangeState(boss1.rangeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
