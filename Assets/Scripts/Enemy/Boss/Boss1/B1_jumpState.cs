using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_jumpState : DodgeState
{
    private Boss1 enemy;
    public B1_jumpState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_dodgeState dodgeData, Boss1 enemy) : base(entity, stateMachine, animBoolName, dodgeData)
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
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }
        else if (isPlayerInMaxAgroRange && !performCloseRangeAction)
        {
            stateMachine.ChangeState(enemy.rangeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
