using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;
    protected float startTime;
    public string animBoolName;
    protected bool isAnimationFinished;
    protected bool isExitingState;
    protected Core core;
   
    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData,string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
        core = player.core;
    }
    public virtual void  Enter()
    {
        Dochecks();
        startTime = Time.time;
        isAnimationFinished = false;
        player.anim.SetBool(animBoolName, true);
        isExitingState = false;

    }
    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
        isExitingState = true;

    }
    public virtual void LogicUpdate()
    {
    }
    public virtual void PhysicsUpdate()
    {
        Dochecks();
    }
    public virtual void Dochecks(){}
    public virtual void AnimationTrigger() { }
    public virtual void AnimationFinishedTrigger() => isAnimationFinished = true;

}
