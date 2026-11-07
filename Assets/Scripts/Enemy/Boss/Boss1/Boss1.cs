using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : Entity
{
    public B1_idleState idleState { get; private set; }
    public B1_moveState moveState { get; private set; }
    public B1_playerDetectedState playerDetectedState { get; private set; }
    public B1_meleeAttackState meleeAttackState { get; private set; }
    public B1_rangeState rangeState { get; private set; }
    public B1_lookForPlayer lookForPlayerState { get; private set; }
    public B1_stunState stunState { get; private set; }
    public B1_deadState deadState { get; private set; }
    public B1_dodgeState dodgeState { get; private set; }
    public B1_summonState summonState { get; private set; }
    public B1_roarState roarState { get; private set; }

    [SerializeField] private D_MoveState moveData;
    [SerializeField] private D_IdleState idleData;
    [SerializeField] private D_PlayerDetected playerDetecteData;
    [SerializeField] private D_MeleeAttackState meleeAttackData;
    [SerializeField] private D_StunState stunData;
    [SerializeField] private D_RangeAttackState rangeData;
    [SerializeField] private D_lookForPlayer lookForPlayerData;
    [SerializeField] public D_dodgeState dodgeData;
    [SerializeField] private D_DeadState deadData;


    [SerializeField] private Transform meleeAttackPos;
    [SerializeField] private Transform rangAttackPosition;
    [Header("Summon Settings")]
    [SerializeField] private D_SummonState summonStateData;
    [SerializeField] private Transform summonAttackPos;
    [Header("Roar Settings")]
    [SerializeField] private D_RoarState roarData;         
    [SerializeField] private Transform roarAttackPos;
    [Header("Patrol")]
    public float patrolRangeX = 0.1f;
    public float patrolRangeY = 0.1f;
    [SerializeField] private LayerMask whatIsPlayer;
    public override void Awake()
    {
        base.Awake();

        idleState = new B1_idleState(this, stateMachine, "idle", idleData, this);
        moveState = new B1_moveState(this, stateMachine, moveData, "move", this);
        playerDetectedState = new B1_playerDetectedState(this, stateMachine, "playerDetected", playerDetecteData, this);
        meleeAttackState = new B1_meleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPos, meleeAttackData, this);
        rangeState = new B1_rangeState(this, stateMachine, "rangeAttack", rangAttackPosition, rangeData, this);
        lookForPlayerState = new B1_lookForPlayer(this, stateMachine, "lookForPlayer", lookForPlayerData, this);
        stunState = new B1_stunState(this, stateMachine, "stun", stunData, this);
        deadState = new B1_deadState(this, stateMachine, "dead", deadData, this);
        dodgeState = new B1_dodgeState(this, stateMachine, "dodge", dodgeData, this);
        summonState = new B1_summonState(this, stateMachine, "summon", summonAttackPos, summonStateData, this);
        roarState = new B1_roarState(this, stateMachine, "roar", roarAttackPos, roarData, this);
    }
    private void Start()
    {
        stateMachine.Initialize(idleState);

    }
    public override void Damage(float amount)
    {
        //base.Damage(amount); // Chạy hàm tính trừ máu ở Entity base

        //if (isDead)
        //{
        //    stateMachine.ChangeState(deadState); // Ngay lập tức chuyển sang trạng thái chết
        //}
        //else if (isStunned && stateMachine.currentState != stunState)
        //{
        //    stateMachine.ChangeState(stunState);
        //}
        //else if (!CheckPlayerInMinAgroRange())
        //{
        //    lookForPlayerState.SetTurnImmediately(true);
        //    stateMachine.ChangeState(lookForPlayerState);
        //}
        //else if (CheckPlayerInMinAgroRange())
        //{
        //    stateMachine.ChangeState(rangeState);
        //}
    }
    public void PlayMeeleeAttackClip()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.quai1Attack != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.quai1Attack);
        }
    }
    public void PlayRangeAttackClip()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.quai1Attack != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.quai1Attack);
        }
    }
    public void PlayRoarClip()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.roarSound != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.roarSound);
        }

    }
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // 1. PHẠM VI CẬN CHIẾN (Melee Attack) - MÀU XANH LÁ (Green)
        if (meleeAttackPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(meleeAttackPos.position, meleeAttackData.attackRadius);
        }

        // 2. PHẠM VI TẤN CÔNG TẦM XA (Range Attack) - MÀU ĐỎ (Red)
        if (rangAttackPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rangAttackPosition.position, entityData.maxAgroDistance);
        }

        // 3. PHẠM VI TRIỆU HỒI (Summon) - MÀU XANH DƯƠNG (Blue)
        if (summonAttackPos != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(summonAttackPos.position, 1.5f); // Hoặc theo bán kính tùy chỉnh của bạn
        }

        // 4. PHẠM VI HÉT (Roar Stun Wave) - MÀU HỒNG TÍM (Magenta)
        if (roarData != null)
        {
            Gizmos.color = Color.magenta;
            Transform roarPos = roarAttackPos != null ? roarAttackPos : transform;

            // Vẽ đường viền vòng tròn bán kính hét
            Gizmos.DrawWireSphere(roarPos.position, roarData.roarRadius);

            // Tô nền mờ bên trong để dễ nhìn vùng ảnh hưởng
            Gizmos.color = new Color(1f, 0f, 1f, 0.15f);
            Gizmos.DrawSphere(roarPos.position, roarData.roarRadius);
        }

        // 5. PHẠM VI PHÁT HIỆN PLAYER (Agro Range) - MÀU VÀNG (Yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, entityData.minAgroDistance);

        // Reset lại màu mặc định
        Gizmos.color = Color.white;
    }
}
