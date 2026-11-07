using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_meleeAttackState : MeleeAttackState
{
    private Boss1 boss1;
    public B1_meleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState meleeAttackData, Boss1 boss1) : base(entity, stateMachine, animBoolName, attackPosition, meleeAttackData)
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
        boss1.LookAtPlayer();

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
                stateMachine.ChangeState(boss1.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(boss1.lookForPlayerState);
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
