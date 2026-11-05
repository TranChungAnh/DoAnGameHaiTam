using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_MeleeAttack : MeleeAttackState
{
    private Enemy3 enemy3;
    public E3_MeleeAttack(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState meleeAttackData, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, attackPosition, meleeAttackData)
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
        enemy3.LookAtPlayer();

        if (entity.Target != null && attackPosition != null)
        {
            Vector2 dir = (entity.Target.position - entity.transform.position).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            attackPosition.rotation = Quaternion.Euler(0, 0, angle);
        }
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
                stateMachine.ChangeState(enemy3.playerDetectedState);
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

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }
}
