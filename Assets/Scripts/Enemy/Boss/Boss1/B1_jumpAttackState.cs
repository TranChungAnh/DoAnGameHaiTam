using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_jumpAttackState : JumpAttackState
{
    private  Boss1 Boss; 
    public B1_jumpAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_jumpAttackState stateData, Boss1 boss)
        : base(entity, stateMachine, animBoolName, stateData, boss)
    {
        this.Boss = boss; 
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + 1f) // Tuỳ logic
        {
            stateMachine.ChangeState(Boss.idleState);
        }
    }
}
