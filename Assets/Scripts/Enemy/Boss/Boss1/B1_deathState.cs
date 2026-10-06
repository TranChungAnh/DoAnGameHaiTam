using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_deathState : DeadState
{
    private Boss1  enemy;
    public B1_deathState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState deadState, Boss1 enemy) : base(entity, stateMachine, animBoolName, deadState)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
