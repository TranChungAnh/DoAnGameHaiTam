using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : State
{
    protected D_dodgeState dodgeData;
    protected bool isPlayerInMaxAgroRange;
    protected bool performCloseRangeAction;
    protected bool isDodgeOver;
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    private CollectionSenses collectionSenses;
    private CollectionSenses CollectionSenses
    {
        get => collectionSenses ?? core.GetCoreComponent(ref collectionSenses);
    }
    public DodgeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_dodgeState dodgeData) : base(entity, stateMachine, animBoolName)
    {
        this.dodgeData = dodgeData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerInMaxAgroRange = entity.CheckPlayerInMaxAgroRange();
        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
        

    }

    public override void Enter()
    {
        base.Enter();
        isDodgeOver = false;
        entity.SetVelocity(dodgeData.dodgeSpeed, dodgeData.dodgeAngle, -Movement.facingDirection);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(Time.time>= startTime + dodgeData.dodgeTime)
        {
            isDodgeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
