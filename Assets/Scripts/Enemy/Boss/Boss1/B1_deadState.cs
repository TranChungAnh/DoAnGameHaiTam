using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_deadState : DeadState
{
    private Boss1 boss;
    public B1_deadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState deadState, Boss1 boss) : base(entity, stateMachine, animBoolName, deadState)
    {
        this.boss = boss;
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
