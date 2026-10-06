using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected D_MoveState stateData;

    protected bool isDetectingWall;
    protected bool isPlayerInMinAgroRange;

    protected Movement Movement
    {
        get => movement ?? core.GetCoreComponent(ref movement);
    }

    private Movement movement;

    private CollectionSenses collectionSenses;
    private CollectionSenses CollectionSenses
    {
        get => collectionSenses ?? core.GetCoreComponent(ref collectionSenses);
    }

    // Điểm Enemy đang muốn đi tới
    private Vector2 patrolTarget;

    // Đã có điểm đích hay chưa
    private bool hasPatrolTarget;

    // Cờ kiểm soát trạng thái tạm dừng và Cooldown check tường
    private bool isWaitingAtWall = false;
    private bool canCheckWall = true;

    public MoveState(
        Entity entity,
        FiniteStateMachine stateMachine,
        D_MoveState stateData,
        string animBoolName
    ) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        // Chỉ check tường khi hết thời gian Cooldown bước ra khỏi tường
        if (canCheckWall)
        {
            isDetectingWall = CollectionSenses.CheckIfToucingWall();
        }
        else
        {
            isDetectingWall = false;
        }

        // Kiểm tra Player
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();

        hasPatrolTarget = false;
        isWaitingAtWall = false;
        canCheckWall = true;

        // Chọn điểm di chuyển mới
        SetNewPatrolTarget();

        // Tính hướng từ Enemy tới điểm đích
        Vector2 direction =
            (patrolTarget - (Vector2)entity.transform.position).normalized;

        // Di chuyển
        Movement?.SetVelocity(
            stateData.movementSpeed,
            direction
        );
    }

    public override void Exit()
    {
        base.Exit();

        Movement?.SetVelovityZero();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Nếu đang trong thời gian đứng im thì không chạy logic tiếp theo
        if (isWaitingAtWall)
            return;

        if (!hasPatrolTarget)
        {
            SetNewPatrolTarget();
        }

        Vector2 direction = (patrolTarget - (Vector2)entity.transform.position).normalized;

        // 1. Cập nhật Vận tốc di chuyển
        Movement?.SetVelocity(stateData.movementSpeed, direction);

        // 2. Cập nhật Animation 8 hướng
        entity.SetAnimationDirection(direction);

        // Đã tới điểm đích
        if (Vector2.Distance(entity.transform.position, patrolTarget) < 0.15f)
        {
            Movement?.SetVelovityZero();
            hasPatrolTarget = false;
        }

        if (isDetectingWall && canCheckWall)
        {
            hasPatrolTarget = false; // Reset target cũ ngay khi va tường
            entity.StartCoroutine(WaitAndChangeDirectionRoutine(0.5f));
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private IEnumerator WaitAndChangeDirectionRoutine(float waitTime)
    {
        isWaitingAtWall = true;
        canCheckWall = false; // Tạm khóa check tường

        // Lấy hướng vừa di chuyển đâm vào tường để tính hướng ngược lại
        Vector2 currentDir = Movement != null && Movement.CurrentVelocity != Vector2.zero
            ? Movement.CurrentVelocity.normalized
            : (patrolTarget - (Vector2)entity.transform.position).normalized;

        // 1. Dừng lại tại chỗ
        Movement?.SetVelovityZero();

        // 2. Đứng im chờ
        yield return new WaitForSeconds(waitTime);

        // 3. Đổi hướng quay ngược lại 180 độ
        Vector2 reverseDirection = -currentDir;

        // Đặt patrolTarget mới nằm ở phía ngược lại
        patrolTarget = (Vector2)entity.transform.position + reverseDirection * 3f;
        hasPatrolTarget = true;

        // 4. Áp dụng ngay lực di chuyển và hướng Animation mới
        Movement?.SetVelocity(stateData.movementSpeed, reverseDirection);
        entity.SetAnimationDirection(reverseDirection);

        isWaitingAtWall = false;

        // 5. Đợi 0.3s cho Enemy bước ra khỏi vùng va chạm của tường rồi mới cho phép check tường lại
        yield return new WaitForSeconds(0.3f);
        canCheckWall = true;
    }

    private void SetNewPatrolTarget()
    {
        Vector2 center = entity.transform.position;
        float rangeX = 3f;
        float rangeY = 3f;

        // Lấy thông số đi tuần tùy theo từng loại Enemy
        if (entity is Enemy1 e1)
        {
            center = e1.patrolCenter;
            rangeX = e1.patrolRangeX;
            rangeY = e1.patrolRangeY;
        }
        else if (entity is Enemy2 e2)
        {
            // Enemy2 lấy vị trí hiện tại (hoặc e2.transform.position) làm tâm patrol Center
            center = e2.transform.position;
            rangeX = e2.patrolRangeX;
            rangeY = e2.patrolRangeY;
        }

        // Tính điểm đến ngẫu nhiên trong khoảng patrolRangeX và patrolRangeY
        patrolTarget = new Vector2(
            center.x + Random.Range(-rangeX, rangeX),
            center.y + Random.Range(-rangeY, rangeY)
        );

        hasPatrolTarget = true;
    }
}