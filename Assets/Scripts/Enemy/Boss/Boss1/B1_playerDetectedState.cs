using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_playerDetectedState : PlayerDetectedState
{
    private Boss1 boss1;
    public B1_playerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected detectedData, Boss1 boss1) : base(entity, stateMachine, animBoolName, detectedData)
    {
        this.boss1 = boss1;
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

        // 1. Ưu tiên xử lý cận chiến hoặc né đòn trước khi Player áp sát đánh trúng
        if (performCloseRangeAction)
        {
            if (Time.time >= boss1.dodgeState.startTime + boss1.dodgeData.dodgeCoolDown)
            {
                stateMachine.ChangeState(boss1.dodgeState);
            }
            else
            {
                stateMachine.ChangeState(boss1.meleeAttackState);
            }
        }
        // 2. Nếu ở khoảng cách tầm xa, cho phép Boss có cơ hội Hét (nếu đã hồi chiêu) hoặc Bắn đạn
        else if (performLongRangeAction)
        {
            // Thêm tỷ lệ ngẫu nhiên hoặc kiểm tra cooldown Roar để Boss không bị spam hét liên tục kẹt cứng
            if (boss1.roarState.CanUseRoar() && Random.value < 0.4f) // 40% cơ hội hét khi đủ điều kiện tầm xa
            {
                stateMachine.ChangeState(boss1.roarState);
            }
            else
            {
                stateMachine.ChangeState(boss1.rangeState);
            }
        }
        // 3. Mất dấu Player thì tìm kiếm
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(boss1.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}