using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_StunSate : StunState
{
    public Enemy1 enemy;
    public E1_StunSate(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stundata, Enemy1 enemy) : base(entity, stateMachine, animBoolName, stundata)
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
        if (isStunTimeOver) 
        {
            if (performCloseRangeAction)
            {
                stateMachine.ChangeState(enemy.meleeAttackState);
            }
            else if(isPlayerInMinArgoRange)
            {
                stateMachine.ChangeState(enemy.chargeState); 
            }
            else
            {
                enemy.lookForPlayerState.SetTurnImmediately(true);
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
