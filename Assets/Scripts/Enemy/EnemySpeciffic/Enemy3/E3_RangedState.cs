using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_RangedState : RangedState
{
    protected Enemy3 enemy3;

    public E3_RangedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangeAttackState rangeData, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, attackPosition, rangeData)
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
        //if (isAnimationFinish)
        //{
        //    if (isPlayerInMinArgoRange)
        //    {
        //        stateMachine.ChangeState(enemy2.playerDetectedState);
        //    }
        //    else
        //    {
        //        stateMachine.ChangeState(enemy2.lookForPlayerState);
        //    }
        //}
        if (isAnimationFinish)
        {
            if (isPlayerInMinArgoRange)
            {
                if (enemy3.poisonZoneState.CanUsePoisonZone())
                {
                    stateMachine.ChangeState(enemy3.poisonZoneState);
                }
                else
                {
                    stateMachine.ChangeState(enemy3.playerDetectedState);
                }
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
