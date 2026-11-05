using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_playerDetectedState : PlayerDetectedState
{
    private Enemy2 enemy2;
    public E2_playerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected detectedData, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, detectedData)
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
        if (performCloseRangeAction)
        {
            if (Time.time >= enemy2.dodgeState.startTime + enemy2.dodgeData.dodgeCoolDown)
            {
                stateMachine.ChangeState(enemy2.dodgeState);
            }
            else
            {
                stateMachine.ChangeState(enemy2.meleeAttackState);
            }
        }
        else if (performLongRangeAction)
        {
            stateMachine.ChangeState(enemy2.rangeState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy2.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
