using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedState : AttackState
{
    protected D_RangeAttackState rangeData;
    protected GameObject projectTitle;
    protected projectTitle projectTitleScrip;

    public RangedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangeAttackState rangeData) : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.rangeData = rangeData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        // 1. Cập nhật Target và ép Enemy quay mặt về phía Player ngay khi vừa vào trạng thái bắn
        entity.CheckPlayerInMinAgroRange();
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

        // 2. Liên tục cập nhật hướng quay mặt theo Player trong lúc đang thực hiện animation bắn
        entity.LookAtPlayer();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        // Ép Enemy cập nhật lại Target Player ngay thời điểm tung đòn bắn
        entity.CheckPlayerInMinAgroRange();
        entity.LookAtPlayer();

        // 1. Tính toán hướng bắn về phía Player
        Vector2 targetDirection = entity.LastFacingDirection;
        Quaternion shootRotation = attackPosition.rotation;

        if (entity.Target != null)
        {
            targetDirection = (entity.Target.position - attackPosition.position).normalized;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            shootRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // 2. Tạo quả cầu
        projectTitle = GameObject.Instantiate(rangeData.projectile, attackPosition.position, shootRotation);

        projectTitleScrip = projectTitle.GetComponent<projectTitle>(); 
        if (projectTitleScrip == null)
        {
            Debug.LogError("projectTitle prefab is missing the 'projectTitle' script!");
            return;
        }

        projectTitleScrip.FireProjectile(
            rangeData.projecTileSpeed,
            rangeData.projectTitleTraveDistance,
            rangeData.projectTitleDamage,
            targetDirection
        );
    }
}