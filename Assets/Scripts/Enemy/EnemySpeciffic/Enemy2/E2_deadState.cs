using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_deadState : DeadState
{
    private Enemy2 enemy2;
    public E2_deadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState deadState, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, deadState)
    {
        this.enemy2 = enemy2;
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
