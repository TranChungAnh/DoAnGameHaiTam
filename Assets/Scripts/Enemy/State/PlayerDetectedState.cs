using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enemy phát hiện ra player
public class PlayerDetectedState : State
{
    protected Movement Movement
    {
        get => movement ?? core.GetCoreComponent(ref movement);
    }

    private Movement movement;

    protected D_PlayerDetected detectedData;
    protected bool isPlayerInMinAgrorange;
    protected bool isPlayerInMaxAgroRange;
    protected bool performLongRangeAction; // có thực hiện hành động tấn công tầm xa
    protected bool performCloseRangeAction; // hành động cận chiến

    public PlayerDetectedState(
        Entity entity,
        FiniteStateMachine stateMachine,
        string animBoolName,
        D_PlayerDetected detectedData
    ) : base(entity, stateMachine, animBoolName)
    {
        this.detectedData = detectedData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMaxAgroRange = entity.CheckPlayerInMaxAgroRange();
        isPlayerInMinAgrorange = entity.CheckPlayerInMinAgroRange();
        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
    }

    public override void Enter()
    {
        base.Enter();

        performLongRangeAction = false;

        // Dừng hoàn toàn
        Movement?.SetVelocity(Vector2.zero);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Dừng hoàn toàn
        Movement?.SetVelocity(Vector2.zero);

        if (Time.time >= startTime + detectedData.longRangeActionTime)
        {
            performLongRangeAction = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}