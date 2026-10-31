using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    // Thuộc tính công khai để các script khác kiểm tra trạng thái choáng
    public bool IsStunned { get; private set; }

    [Header("Cấu hình Âm thanh Di chuyển")]
    [SerializeField] private float footstepInterval = 0.4f;
    private float footstepTimer;

    private void Awake()
    {
        animator=GetComponent<Animator>();
    }

<<<<<<< Updated upstream
    public void UpdateMomentAnimation(Vector2 facingDirection,bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isIdle", !isMoving);
        animator.SetFloat("MoveY", facingDirection.y);
        animator.SetFloat("MoveX", facingDirection.x);

=======
    private void Update()
    {
        // Đếm ngược thời gian bước chân khi đang di chuyển
        if (footstepTimer > 0)
        {
            footstepTimer -= Time.deltaTime;
        }
    }

    public void UpdateMomentAnimation(Vector2 facingDirection, bool isMoving)
    {
        // ĐANG BỊ STUN HOẶC TẤN CÔNG -> không cho Movement ghi đè animation
        if (IsStunned || animator.GetBool("attack"))
        {
            return;
        }

        animator.SetBool("move", isMoving);
        animator.SetBool("idle", !isMoving);

        animator.SetFloat("MoveY", facingDirection.y);
        animator.SetFloat("MoveX", facingDirection.x);

        // 🔊 XỬ LÝ PHÁT TIẾNG BƯỚC CHÂN KHI ĐI CHUYỂN
        if (isMoving)
        {
            PlayFootstepSound();
        }
>>>>>>> Stashed changes
    }
    public void SetIdleAnimation(Vector2 facingDirection)
    {
<<<<<<< Updated upstream
        animator.SetBool("isMoving", false);
        animator.SetBool("isIdle", true);
        animator.SetFloat("MoveY", facingDirection.y);
        animator.SetFloat("MoveX", facingDirection.x);
    }
=======
        if (IsStunned || animator.GetBool("attack"))
        {
            return;
        }

        animator.SetBool("move", false);
        animator.SetBool("idle", true);

        animator.SetFloat("MoveY", facingDirection.y);
        animator.SetFloat("MoveX", facingDirection.x);
    }

  
    public void SetAttackAnimation(bool isAttacking, Vector2 facingDirection)
    {
        if (IsStunned) return;

        animator.SetBool("attack", isAttacking);

        if (isAttacking)
        {
            animator.SetBool("move", false);
            animator.SetBool("idle", false);

            animator.SetFloat("MoveY", facingDirection.y);
            animator.SetFloat("MoveX", facingDirection.x);
        }
        else
        {
            animator.SetBool("attack", false);
            animator.SetBool("move", false);
            animator.SetBool("idle", true);

            animator.SetFloat("MoveY", facingDirection.y);
            animator.SetFloat("MoveX", facingDirection.x);
        }
    }

>>>>>>> Stashed changes
    public void PlayerActionAnimation(string actionName)
    {
        if (IsStunned) return;
        animator.SetTrigger(actionName);
<<<<<<< Updated upstream
=======
    }

    public void TriggerStun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        IsStunned = true; // Đánh dấu trạng thái bị stun

        // Bật trạng thái Stun trên Animator và ngắt các hoạt ảnh khác
        animator.SetBool("isStunned", true);
        animator.SetBool("move", false);
        animator.SetBool("idle", false);
        animator.SetBool("attack", false);

        // Đợi hết thời gian choáng
        yield return new WaitForSeconds(duration);

        // Phục hồi lại trạng thái bình thường
        animator.SetBool("isStunned", false);
        IsStunned = false;
    }


    private void PlayFootstepSound()
    {
        // Kiểm tra xem SoundManager đã có trong Scene chưa
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("⚠ Cảnh báo: Không tìm thấy SoundManager.Instance trong Scene! Vui lòng kiểm tra lại.");
            return;
        }

        if (footstepTimer <= 0)
        {
            if (SoundManager.Instance.diTrenCat != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.diTrenCat);

            }
           
            footstepTimer = footstepInterval;
        }
>>>>>>> Stashed changes
    }
}
