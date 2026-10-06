using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DeadState : State
{
    protected D_DeadState deadData;

    public DeadState(
        Entity entity,
        FiniteStateMachine stateMachine,
        string animBoolName,
        D_DeadState deadState
    ) : base(entity, stateMachine, animBoolName)
    {
        this.deadData = deadState;
    }

    public override void Enter()
    {
        base.Enter();

        Collider2D col = entity.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 2. Dừng hẳn di chuyển của quái
        entity.SetVelocity(0f, Vector2.zero, 0);
        // 2. Kiểm tra D_DeadState ScriptableObject có bị NULL không
        if (deadData == null)
        {
            Debug.LogError($"[DEAD STATE ERROR] -> deadData (D_DeadState) trên {entity.gameObject.name} đang bị NULL!");
            return;
        }

        // 3. Kiểm tra & Tạo Hiệu ứng máu
        if (deadData.deadBloodParticles != null)
        {
            GameObject.Instantiate(
                deadData.deadBloodParticles,
                entity.transform.position,
                entity.transform.rotation
            );
            Debug.Log($"[DEAD STATE LOG] -> Đã tạo deadBloodParticles cho {entity.gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[DEAD STATE WARNING] -> deadBloodParticles chưa được gán (NULL) trong ScriptableObject của {entity.gameObject.name}!");
        }

        // 4. Kiểm tra & Tạo Hiệu ứng mảnh vỡ (Chunk Particles)
        if (deadData.deadChunkParticles != null)
        {
            GameObject bloodObj = GameObject.Instantiate(
                deadData.deadChunkParticles,
                entity.transform.position,
                entity.transform.rotation
            );
            Debug.Log($"[DEAD STATE LOG] -> Đã tạo deadChunkParticles THÀNH CÔNG cho {entity.gameObject.name} (Tên Object: {bloodObj.name})");
        }
        else
        {
            Debug.LogError($"[DEAD STATE ERROR] -> deadChunkParticles bị NULL! Kiểm tra ô deadChunkParticles trong ScriptableObject {deadData.name}!");
        }

        // Chờ animation chết chạy xong rồi ẩn
        entity.StartCoroutine(HideAfterDeath());
    }

    private IEnumerator HideAfterDeath()
    {
        Debug.Log($"[DEAD STATE LOG] -> Bắt đầu đếm ngược 2s để ẩn {entity.gameObject.name}");
        yield return new WaitForSeconds(2f);

        // Chỉ ẩn, không Destroy
        entity.gameObject.SetActive(false);
        Debug.Log($"[DEAD STATE LOG] -> Đã SetActive(false) cho {entity.gameObject.name}");
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