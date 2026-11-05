using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_meleeAttackState : MeleeAttackState
{
    private Boss1 enemy;
    public B1_meleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState meleeAttackData, Boss1 enemy) : base(entity, stateMachine, animBoolName, attackPosition, meleeAttackData)
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

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinish)
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

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }
}
