using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarState : AttackState
{
    protected D_RoarState roarData;
    protected float lastRoarTime = -Mathf.Infinity; 

    private bool isRoarWaveTriggered; 

    public RoarState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RoarState roarData)
        : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.roarData = roarData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        lastRoarTime = Time.time;
        isRoarWaveTriggered = false;

        entity.CheckPlayerInMinAgroRange();
        entity.LookAtPlayer();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        entity.LookAtPlayer();

          if (!isRoarWaveTriggered && Time.time >= startTime + 0.3f)
        {
            ExecuteRoarWave();
            isRoarWaveTriggered = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        if (isRoarWaveTriggered) return;

        ExecuteRoarWave();
        isRoarWaveTriggered = true;
    }

    private void ExecuteRoarWave()
    {
        // Cập nhật lại vị trí / hướng trước khi phát sóng âm
        entity.CheckPlayerInMinAgroRange();
        entity.LookAtPlayer();

        Vector2 centerPos = attackPosition != null ? attackPosition.position : entity.transform.position;
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(centerPos, roarData.roarRadius, roarData.whatIsPlayer);


        foreach (Collider2D playerCollider in hitPlayers)
        {
            if (playerCollider != null)
            {
                PlayerAnimation playerAnim = playerCollider.GetComponent<PlayerAnimation>();
                if (playerAnim != null)
                {
                    playerAnim.TriggerStun(roarData.stunDuration);
                }
                else
                {
                    Debug.LogWarning($"[RoarState] Tìm thấy collider {playerCollider.name} nhưng thiếu component PlayerAnimation!");
                }
            }
        }
    }

    // Hàm kiểm tra xem đã hết thời gian hồi chiêu Hét chưa
    public bool CanUseRoar()
    {
        return Time.time >= lastRoarTime + roarData.roarCooldown;
    }
}