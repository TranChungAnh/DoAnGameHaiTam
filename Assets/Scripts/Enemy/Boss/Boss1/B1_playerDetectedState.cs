using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_playerDetectedState : PlayerDetectedState
{
    private Boss1  enemy;
    public B1_playerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected detectedData, Boss1 enemy) : base(entity, stateMachine, animBoolName, detectedData)
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
        if (performCloseRangeAction)
        {
            if (Time.time >= enemy.jumpAttackState.startTime + enemy.dodgeData.dodgeCoolDown)
            {
                stateMachine.ChangeState(enemy.jumpAttackState);
            }
            else
            {
                stateMachine.ChangeState(enemy.meleeAttackState);
            }
        }
        else if (performLongRangeAction)
        {
            stateMachine.ChangeState(enemy.rangeState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
