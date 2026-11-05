using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState
{
    protected D_MeleeAttackState meleeAttackData;

    public MeleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState meleeAttackData) : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.meleeAttackData = meleeAttackData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        entity.LookAtPlayer();
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        if (meleeAttackData == null)
        {
            Debug.LogError("Melee attack data is not set.");
            return;
        }

        Vector2 facingDir = entity.LastFacingDirection;
        Vector3 actualAttackPos = entity.transform.position + (Vector3)(facingDir * meleeAttackData.attackRadius);

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(actualAttackPos, meleeAttackData.attackRadius, meleeAttackData.whatIsPlayer);

        foreach (Collider2D collider in detectedObjects)
        {
            // CHÚ Ý CHỖ NÀY: Phải có 
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(meleeAttackData.attackDamage);
                if (States.Instance != null)
                {
                    float currentHealth = States.Instance.currentHealth;
                    if (currentHealth > 0)
                    {
                        States.Instance.setHealth(currentHealth - meleeAttackData.attackDamage);
                    }
                }
                else
                {
                    Debug.LogError("States.Instance is null.");
                }
            }

            // CHÚ Ý CHỖ NÀY: Phải có 
            IKnockBack knockbackable = collider.GetComponent<IKnockBack>();
            if (knockbackable != null)
            {
                int direction = (collider.transform.position.x >= entity.transform.position.x) ? 1 : -1;
                knockbackable.KnockBack(meleeAttackData.knockbackAngle, meleeAttackData.knockbackForce, direction);
            }
        }
    }
}