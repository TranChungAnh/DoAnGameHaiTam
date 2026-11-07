using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_summonState : SummonState
{
    private Boss1 boss1;

    public B1_summonState(
        Entity entity,
        FiniteStateMachine stateMachine,
        string animBoolName,
        Transform attackPosition,
        D_SummonState summonStateData,
        Boss1 boss1)
        : base(entity, stateMachine, animBoolName, attackPosition, summonStateData)
    {
        this.boss1 = boss1;
    }

    public override void Enter()
    {
        base.Enter();
        boss1.LookAtPlayer();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isSummonFinished)
        {      
                stateMachine.ChangeState(boss1.rangeState);
        }
    }
}