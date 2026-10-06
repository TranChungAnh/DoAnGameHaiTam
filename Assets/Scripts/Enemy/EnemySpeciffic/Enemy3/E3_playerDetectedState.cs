using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_playerDetectedState : PlayerDetectedState
{
    private Enemy3 enemy3;
    public E3_playerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected detectedData, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, detectedData)
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
        if (performCloseRangeAction)
        {
            if (Time.time >= enemy3.dodgeState.startTime + enemy3.dodgeData.dodgeCoolDown)
            {
                stateMachine.ChangeState(enemy3.dodgeState);
            }
            else
            {
                stateMachine.ChangeState(enemy3.meleeAttackState);
            }
        }
        else if (performLongRangeAction)
        {
            stateMachine.ChangeState(enemy3.rangeState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(enemy3.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
