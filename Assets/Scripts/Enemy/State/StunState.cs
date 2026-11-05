using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
//  lớp gây choáng 
public class StunState : State
{
    protected D_StunState stundata;
    protected bool isStunTimeOver;
    protected bool isMovementStop;
    protected bool performCloseRangeAction;
    protected bool isPlayerInMinArgoRange;
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    private CollectionSenses collectionSenses;
    private CollectionSenses CollectionSenses
    {
        get => collectionSenses ?? core.GetCoreComponent(ref collectionSenses);
    }
    public StunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stundata) : base(entity, stateMachine, animBoolName)
    {
        this.stundata = stundata;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        isMovementStop = false;
        isStunTimeOver = false;
        entity.SetVelocity(stundata.stunKonckBackSpeed, stundata.stunKnockBackAngle, entity.lastDamageDirection);
        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
        isPlayerInMinArgoRange = entity.CheckPlayerInMinAgroRange();
        entity.SetVelocity(stundata.stunKonckBackSpeed, stundata.stunKnockBackAngle, entity.lastDamageDirection);
    }

    public override void Exit()
    {
        base.Exit();
        entity.ResetStunResistance();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(Time.time>=startTime + stundata.stunTime)
        {
            isStunTimeOver = true;
        }
        if(Time.time>=startTime+stundata.stunKnockBackTime && !isMovementStop)
        {
            isMovementStop = true;
            Movement?.SetVelocityX(0f);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
