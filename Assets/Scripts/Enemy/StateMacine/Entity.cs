using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Kế thừa thêm IDamageable và IKnockBack
public class Entity : MonoBehaviour, IDamageable, IKnockBack
{
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    private CollectionSenses collectionSenses;
    private CollectionSenses CollectionSenses
    {
        get => collectionSenses ?? core.GetCoreComponent(ref collectionSenses);
    }

    public Animator anim { get; private set; }

    public FiniteStateMachine stateMachine;
    private Vector2 velocityWorkspace;

    public D_Entity entityData;

    [SerializeField]
    private Transform wallCheck;

    [SerializeField]
    public Transform playerCheck;

    public AnimationToStateMachine atsm { get; private set; }

    private float currentHealth;
    public int lastDamageDirection { get; private set; }
    public Core core { get; private set; }

    private float currentStunResistance;
    private float lastDamageTime;

    public bool isStunned;
    public bool isDead;
    public Transform Target { get; private set; }

    public virtual void Awake()
    {
        core = GetComponentInChildren<Core>();


        if (core == null)
        {
        }

        currentHealth = entityData.maxHealth;
        currentStunResistance = entityData.stunResistance;
        anim = GetComponent<Animator>();

        stateMachine = new FiniteStateMachine();

        atsm = GetComponent<AnimationToStateMachine>();

    }
    public virtual void Update()
    {
        core.LogicUpdate();

        //anim.SetFloat("yVelocity", Movement.rb.velocity.y);

        stateMachine.currentState.LogicUpdate();

        if (Time.time >= lastDamageTime + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }

    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual bool CheckPlayerInMinAgroRange()
    {
        Collider2D player = Physics2D.OverlapCircle(
            playerCheck.position,
            entityData.minAgroDistance,
            entityData.whatIsPlayer
        );

        if (player != null)
        {
            Target = player.transform; // Cập nhật Target mới nhất
            return true;
        }

        Target = null; // Reset Target khi Player chạy mất
        return false;
    }

    // Phạm vi enemy tiếp tục đuổi Player (Hình tròn Max Agro)
    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.OverlapCircle(
            playerCheck.position,
            entityData.maxAgroDistance,
            entityData.whatIsPlayer
        );
    }

    // Phạm vi tấn công cận chiến (Hình tròn Close Range Action)
    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.OverlapCircle(
            playerCheck.position,
            entityData.closeRangeActionDistance,
            entityData.whatIsPlayer
        );
    }

     public virtual void Damage(float amount)
    {
        lastDamageTime = Time.time;
        currentHealth -= amount;

        // Đồng bộ sát thương sang component States cũ (để Player & Enemy nhận đúng)
        States stats = GetComponentInChildren<States>();
        if (stats != null)
        {
            stats.DecreaseHealth(amount);
        }

        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }
    public virtual void KnockBack(Vector2 angle, float strength, int direction)
    {
        if (Movement != null && strength > 0)
        {
            angle.Normalize();
            Vector2 knockbackVelocity = new Vector2(angle.x * strength * direction, angle.y * strength);
            Movement.rb.velocity = knockbackVelocity;
        }
    }

    // ==========================================================
    // DAMAGE GIỮ NGUYÊN CODE CŨ CỦA BẠN
    // ==========================================================

    public virtual void DamageHop(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();

        velocityWorkspace.Set(
            angle.x * velocity * direction,
            angle.y * velocity
        );

        Movement.rb.velocity = velocityWorkspace;
    }

    public void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = entityData.stunResistance;
    }

    public virtual void SetVelocity(
        float velocity,
        Vector2 angle,
        int direction
    )
    {
        angle.Normalize();

        velocityWorkspace.Set(
            angle.x * velocity * direction,
            angle.y * velocity
        );

        Movement.rb.velocity = velocityWorkspace;
    }
    public virtual void LookAtPlayer()
    {
        Transform targetTransform = Target;

        if (targetTransform == null)
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(
                playerCheck.position,
                entityData.maxAgroDistance,
                entityData.whatIsPlayer
            );
            if (playerCollider != null) targetTransform = playerCollider.transform;
        }

        if (targetTransform != null)
        {
            // 1. Tính Vector hướng từ Enemy đến Player
            Vector2 dir = (targetTransform.position - transform.position).normalized;

            if (dir != Vector2.zero)
            {
                // 2. Tính góc (độ)
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;

                // 3. Quy đổi ra 1 trong 8 hướng chuẩn
                int index = Mathf.FloorToInt((angle + 22.5f) / 45f) % 8;

                Vector2 dir8 = index switch
                {
                    0 => Vector2.right,
                    1 => new Vector2(1, 1).normalized,      // Phải - Trên
                    2 => Vector2.up,
                    3 => new Vector2(-1, 1).normalized,     // Trái - Trên
                    4 => Vector2.left,
                    5 => new Vector2(-1, -1).normalized,    // Trái - Dưới
                    6 => Vector2.down,
                    7 => new Vector2(1, -1).normalized,     // Phải - Dưới
                    _ => Vector2.right
                };

                SetAnimationDirection(dir8);
            }
        }
    }
    // Thêm biến lưu hướng nhìn cuối cùng
    public Vector2 LastFacingDirection { get; private set; } = Vector2.down;

    public void SetAnimationDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            direction.Normalize();
            LastFacingDirection = direction;

            anim.SetFloat("moveX", direction.x);
            anim.SetFloat("moveY", direction.y);
        }
        else
        {
            // Khi đứng yên (direction = zero), giữ nguyên hướng nhìn gần nhất cho Idle
            anim.SetFloat("moveX", LastFacingDirection.x);
            anim.SetFloat("moveY", LastFacingDirection.y);
        }
    }
    public virtual void Damage1(WeaponAttackDetails attackDetails, Vector2 attackerPosition)
    {
        lastDamageTime = Time.time;
        currentHealth -= attackDetails.damageAmount;

        // 1. Tính Vector hướng đẩy lùi Top-Down (Tất cả góc 360 độ)
        Vector2 knockbackDirection = ((Vector2)transform.position - attackerPosition).normalized;

        // 2. Áp dụng lực đẩy lùi Knockback theo Vector 2D (Top-Down)
        if (Movement != null && attackDetails.knockbackForce > 0)
        {
            // Tính Vector vận tốc đẩy lùi kết hợp lực knockbackForce
            Vector2 knockbackVelocity = knockbackDirection * attackDetails.knockbackForce;
            Movement.rb.velocity = knockbackVelocity;
        }

        // 3. Đánh dấu đã chết khi hết máu
        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public virtual void OnDrawGizmos()
    {
        if (entityData == null) return;

        // 1. Check Tường (Vẫn giữ Raycast đường thẳng theo hướng quay)
        if (wallCheck != null)
        {
            Vector2 facingDir = Vector2.right;
            if (Application.isPlaying && Movement != null)
            {
                facingDir = Movement.FacingDirection;
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                wallCheck.position,
                wallCheck.position + (Vector3)(facingDir * entityData.wallCheckDistance)
            );
        }

        // 2. Gizmos Vòng Tròn Phát Hiện Player
        if (playerCheck != null)
        {
            // Tầm tấn công cận chiến -> Vòng tròn Đỏ
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerCheck.position, entityData.closeRangeActionDistance);

            // Tầm phát hiện tối thiểu (Phát hiện) -> Vòng tròn Vàng
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerCheck.position, entityData.minAgroDistance);

            // Tầm duy trì theo đuổi tối đa -> Vòng tròn Cam
            Gizmos.color = new Color(1f, 0.5f, 0f); // Orange
            Gizmos.DrawWireSphere(playerCheck.position, entityData.maxAgroDistance);
        }
    }
}