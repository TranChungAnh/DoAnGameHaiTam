using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy1 : Entity
{
    public E1_IdleState idleState { get; set; }
    public E1_MoveState moveState { get; set; }
    public E1_DetectedState playerDetectedState { get; private set; }
    public E1_ChangeState chargeState { get; private set; }
    public E1_LookForPlayerState lookForPlayerState { get; private set; }
    public E1_MeleeAttackState meleeAttackState { get; private set; }
    public E1_StunSate stunState { get; private set; }
    public E1_DeadState deadState { get; private set; }

    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_PlayerDetected playerDetectedData;
    [SerializeField]
    private D_ChargeState chargeStateData;
    [SerializeField]
    private D_lookForPlayer lookforPlayerData;
    [SerializeField]
    private D_MeleeAttackState meleeAttackData;
    [SerializeField]
    private D_StunState stunStateData;
    [SerializeField]
    private D_DeadState deadData;
    [SerializeField]
    private Transform attackPosition;
    [Header("Patrol")]
    public float patrolRangeX = 5f;
    public float patrolRangeY = 3f;

    public Vector2 patrolCenter { get; private set; }
    public override void Awake()
    {
        base.Awake();
        moveState = new E1_MoveState(this, stateMachine,  moveStateData, "move", this);
        idleState = new E1_IdleState(this, stateMachine, "idle", this, idleStateData);
        playerDetectedState = new E1_DetectedState(this, stateMachine, "playerDetected", playerDetectedData, this);
        chargeState = new E1_ChangeState(this, stateMachine, "charge", chargeStateData, this);
        lookForPlayerState = new E1_LookForPlayerState(this, stateMachine, "lookForPlayer", lookforPlayerData, this);
        meleeAttackState = new E1_MeleeAttackState(this, stateMachine, "meleeAttack", attackPosition, meleeAttackData, this);
        stunState = new E1_StunSate(this, stateMachine, "stun", stunStateData, this);
        deadState = new E1_DeadState(this, stateMachine, "dead", deadData, this);
    }
    private void Start()
    {
        stateMachine.Initialize(moveState);
        patrolCenter = transform.position;
        stateMachine.Initialize(moveState);
    }
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireSphere(attackPosition.position, meleeAttackData.attackRadius);
    }

    //public override void Damage1(AttackDetails attackDetails)
    //{
    //    base.Damage1(attackDetails);
    //    if (isDead)
    //    {
    //        stateMachine.ChangeState(deadState);
    //    }
    //    else if (isStunned && stateMachine.currentState != stunState) ;
    //    {
    //        stateMachine.ChangeState(stunState);
    //    }

    //}
    public void HideEnemy()
    {
        gameObject.SetActive(false);
    }
}
