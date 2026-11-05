using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForPlayerState : State
{
    protected D_lookForPlayer lookForPlayerData;
    protected bool turnImmediately;// có lật ngay lập tức không 
    protected bool isPlayerInMinAgroRange;
    protected bool isAllTurnsDone;// xong tất cả các lần xoay
    protected bool isAllTurnsTimeDone;
    protected float lastTurnTime;
    protected int amountOfTurnsDone;// số lần enemy đã thực hiện trong quá trình tìm kiếm 
    public LookForPlayerState(Entity entity, FiniteStateMachine stateMachine, string animBoolName,D_lookForPlayer lookForPlayer) : base(entity, stateMachine, animBoolName)
    {
        this.lookForPlayerData = lookForPlayer;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();
        isAllTurnsDone = false;
        isAllTurnsTimeDone = false;
        lastTurnTime = startTime;
        amountOfTurnsDone = 0;
        Movement?.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        Movement?.SetVelocityX(0f);
        if (turnImmediately)
        {
           Movement?.Flip();
            lastTurnTime = Time.time;
            amountOfTurnsDone++;
            turnImmediately = false;
        }
        else if(Time.time >= lastTurnTime + lookForPlayerData.timeBetweenTurns && !isAllTurnsDone)
        {
           Movement?.Flip();
            lastTurnTime = Time.time;
            amountOfTurnsDone++;
        }
        if(amountOfTurnsDone>= lookForPlayerData.amountOfTurns)
        {
            isAllTurnsDone = true;
        }
        if(Time.time>= lastTurnTime + lookForPlayerData.timeBetweenTurns && isAllTurnsDone)
        {
            isAllTurnsTimeDone = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    // quay lại ngay lập tưc
    public void SetTurnImmediately(bool flip)
    {
        turnImmediately = flip;
    }
}
