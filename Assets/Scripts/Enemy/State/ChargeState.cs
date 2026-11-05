using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// trạng thái lao tới (Charge) của kẻ địch
public class ChargeState : State
{
    protected D_ChargeState chargeData;
    protected bool isPlayerInMinAgroRange;
    protected bool isDetectingWall;
    protected bool isChargeTimeOver;
    protected bool performCloseRangeAction;

    // 1. THÊM BẤT BỘC: Biến lưu Vector2 hướng lao 360 độ
    protected Vector2 chargeDirection;

    protected Movement Movement
    {
        get => movement ?? core.GetCoreComponent(ref movement);
    }

    private Movement movement;

    private CollectionSenses collectionSenses;
    private CollectionSenses CollectionSenses
    {
        get => collectionSenses ?? core.GetCoreComponent(ref collectionSenses);
    }

    public ChargeState(
        Entity entity,
        FiniteStateMachine stateMachine,
        string animBoolName,
        D_ChargeState changeData
    ) : base(entity, stateMachine, animBoolName)
    {
        this.chargeData = changeData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        // Giữ check tường
        isDetectingWall = CollectionSenses.CheckIfToucingWall();

        // Hàm này chạy sẽ tự động cập nhật entity.Target nếu quét thấy Player
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
    }

    public override void Enter()
    {
        base.Enter();

        isChargeTimeOver = false;

        if (entity.Target != null)
        {
            chargeDirection = (entity.Target.position - entity.transform.position).normalized;
        }
        else
        {
            // Dự phòng nếu chưa có Target thì lao theo hướng nhìn gần nhất
            chargeDirection = entity.LastFacingDirection;
        }

        // Cập nhật hướng cho Blend Tree Animator (moveX, moveY)
        entity.SetAnimationDirection(chargeDirection);

        // Lao theo Vector2 hướng vừa tính
        Movement?.SetVelocity(
            chargeData.chargeSpeed,
            chargeDirection
        );
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        entity.LookAtPlayer();
        Movement?.SetVelocity(
            chargeData.chargeSpeed,
            chargeDirection
        );

        if (Time.time >= startTime + chargeData.chargeTime)
        {
            isChargeTimeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}