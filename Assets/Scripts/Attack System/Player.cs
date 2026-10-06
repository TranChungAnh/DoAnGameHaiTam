using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static PlayerInputHandler;

public class Player : MonoBehaviour
{
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }

    public PlayerDashState dashState { get; private set; }
    public PlayerAttackState primaryAttackState { get; private set; }
    public PlayerAttackState secondaryAttackState { get; private set; }
    public PlayerInventory inventory { get; private set; }

    public Core core { get; private set; }

    public Animator anim { get; private set; }
    public BoxCollider2D movementCollider { get; private set; }
    public Rigidbody2D rb { get; private set; }

    [SerializeField]
    private PlayerData playerData;
    public Transform dashDirectionIndicator;
    public float gameStartTime;


    private void Awake()
    {
        core = GetComponentInChildren<Core>();
        stateMachine = new PlayerStateMachine();

        // Chỉ khởi tạo các State dùng cho Top-down (Đứng yên, Di chuyển, Lướt, Tấn công)
        idleState = new PlayerIdleState(this, stateMachine, playerData, "idle");
        moveState = new PlayerMoveState(this, stateMachine, playerData, "move");
        dashState = new PlayerDashState(this, stateMachine, playerData, "dash");
        primaryAttackState = new PlayerAttackState(this, stateMachine, playerData, "attack");
        secondaryAttackState = new PlayerAttackState(this, stateMachine, playerData, "attack");
        gameStartTime = Time.time;

    }


    private void Start()
    {
        anim = GetComponent<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody2D>();
        movementCollider = GetComponent<BoxCollider2D>();
        dashDirectionIndicator = transform.Find("DashDirectionIndicator");

        inventory = GetComponent<PlayerInventory>();

        primaryAttackState.SetWeapon(
            inventory.weapons[(int)CombatInputs.primary]
        );

        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        core.LogicUpdate();
        stateMachine.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    // Callbacks dùng cho Animation Event
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
    public void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishedTrigger();
}