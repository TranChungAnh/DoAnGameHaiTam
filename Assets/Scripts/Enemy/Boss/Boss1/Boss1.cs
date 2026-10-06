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
    public B1_deathState deadState { get; private set; }
    public B1_jumpState jumpState { get; private set; }
    public B1_jumpAttackState jumpAttackState { get; private set; } 

    [SerializeField] private D_MoveState moveData;
    [SerializeField] private D_IdleState idleData;
    [SerializeField] private D_PlayerDetected playerDetecteData;
    [SerializeField] private D_MeleeAttackState meleeAttackData;
    [SerializeField] private D_StunState stunData;
    [SerializeField] private D_RangeAttackState rangeData;
    [SerializeField] private D_lookForPlayer lookForPlayerData;
    [SerializeField] public D_dodgeState dodgeData;
    [SerializeField] private D_DeadState deadData;
    [SerializeField] private D_jumpAttackState jumpAttackData; 

    [SerializeField] private Transform meleeAttackPos;
    [SerializeField] private Transform rangAttackPosition;
    [SerializeField] private bool isJumpAttack;

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
        jumpState = new B1_jumpState(this, stateMachine, "jump", dodgeData, this);
        deadState = new B1_deathState(this, stateMachine, "dead", deadData, this);
        jumpAttackState = new B1_jumpAttackState(this, stateMachine, "jumpAttack", jumpAttackData, this); 
    }

    private void Start()
    {
        stateMachine.Initialize(moveState);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (meleeAttackPos != null)
            Gizmos.DrawWireSphere(meleeAttackPos.position, meleeAttackData.attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, entityData.minAgroDistance);

        if (rangAttackPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rangAttackPosition.position, entityData.maxAgroDistance);
        }

        Gizmos.color = Color.white;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isJumpAttack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isJumpAttack = false;
        }
    }

}
