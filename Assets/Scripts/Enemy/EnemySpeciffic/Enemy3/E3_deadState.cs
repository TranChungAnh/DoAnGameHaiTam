using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_deadState : DeadState
{
    private Enemy3 enemy3;
    public E3_deadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState deadState, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, deadState)
    {
        this.enemy3 = enemy3;
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
