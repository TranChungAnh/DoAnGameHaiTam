using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_PoisonZoneState : PoisonZoneState
{
    private Enemy3 enemy3;

    public E3_PoisonZoneState(
        Entity entity,
        FiniteStateMachine stateMachine,
        string animBoolName,
        GameObject poisonZonePrefab,
        float poisonZoneDuration,
        float poisonZoneCooldown,
        LayerMask whatIsPlayer,
        Enemy3 enemy3)
        : base(
            entity,
            stateMachine,
            animBoolName,
            poisonZonePrefab,
            poisonZoneDuration,
            poisonZoneCooldown,
            whatIsPlayer)
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
        enemy3.LookAtPlayer();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isPoisonZoneFinished)
        {
            stateMachine.ChangeState(enemy3.rangeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}