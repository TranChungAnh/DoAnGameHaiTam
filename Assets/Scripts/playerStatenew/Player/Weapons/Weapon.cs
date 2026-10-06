using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data & Components")]
    [SerializeField] protected SO_weaponData weaponData;
    protected Animator baseAnim;
    protected Animator weaponAnim;

    protected PlayerAttackState attackState;
    protected int attackCount;
    protected Core core;

    // Thêm để điều khiển animation của Player
    private PlayerAnimation playerAnimation;
    protected PlayerAnimation PlayerAnim 
    { 
        get 
        {
            if (playerAnimation == null)
            {
                playerAnimation = GetComponentInParent<PlayerAnimation>();
            }
            return playerAnimation;
        }
    }
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    // Lưu lại hướng tấn công gần nhất để không bị trôi về Right khi combo
    private Vector2 lastAttackDir = Vector2.down;

    protected virtual void Awake()
    {
        baseAnim = transform.Find("Base").GetComponentInChildren<Animator>();
        weaponAnim = transform.Find("Weapon").GetComponentInChildren<Animator>();

        // Lấy PlayerAnimation từ Player
        playerAnimation = GetComponentInParent<PlayerAnimation>();

        gameObject.SetActive(false);
        attackCount = 0;
    }

    public virtual void EnterWeapon()
    {
        if (attackCount >= weaponData.amountOfAttack)
        {
            attackCount = 0;
        }

        gameObject.SetActive(true);

        // 1. Lấy hướng từ Movement
        Vector2 facingDir = Movement != null ? Movement.FacingDirection : Vector2.zero;

        // 2. Xử lý hướng Top-Down
        if (facingDir != Vector2.zero)
        {
            lastAttackDir = facingDir;
        }
        else
        {
            facingDir = lastAttackDir; // Lấy lại hướng đòn đánh trước đó
        }

        // Bảo vệ: Nếu ngay cả lastAttackDir vẫn bằng Zero (lần đầu vào game), gán mặc định hướng Xuống
        if (facingDir == Vector2.zero)
        {
            facingDir = Vector2.down;
        }

        // 3. Ép chuẩn 4 hướng chính (Top-Down: Lên / Xuống / Trái / Phải)
        if (Mathf.Abs(facingDir.y) >= Mathf.Abs(facingDir.x) && facingDir.y != 0)
        {
            facingDir = new Vector2(0, Mathf.Sign(facingDir.y));
        }
        else if (facingDir.x != 0)
        {
            facingDir = new Vector2(Mathf.Sign(facingDir.x), 0);
        }

        // Lưu lại hướng chuẩn vừa tính
        lastAttackDir = facingDir;

        // Reset Trigger/Bool cho Vũ khí
        baseAnim.SetBool("attack", false);
        weaponAnim.SetBool("attack", false);

        // Cập nhật hướng cho Animator Vũ khí
        baseAnim.SetFloat("moveX", facingDir.x);
        baseAnim.SetFloat("moveY", facingDir.y);
        baseAnim.SetInteger("attackCounter", attackCount);

        weaponAnim.SetFloat("moveX", facingDir.x);
        weaponAnim.SetFloat("moveY", facingDir.y);
        weaponAnim.SetInteger("attackCounter", attackCount);

        // Bật animation attack của Vũ khí
        baseAnim.SetBool("attack", true);
        weaponAnim.SetBool("attack", true);

        // Cập nhật Animator của Player
        if (PlayerAnim != null)
        {
            PlayerAnim.SetAttackAnimation(true, facingDir);
        }
    }
   
    public virtual void ExitWeapon()
    {
        // Tắt attack của Weapon
        baseAnim.SetBool("attack", false);
        weaponAnim.SetBool("attack", false);

        attackCount++;


        if (playerAnimation != null)
        {
            playerAnimation.SetAttackAnimation(false, lastAttackDir);
        }

        gameObject.SetActive(false);
    }

    #region Animation Triggers

    public virtual void AnimationFinishedTrigger()
    {
        attackState.AnimationFinishedTrigger();
        Debug.Log("AnimationFinishedTrigger đã được gọi!");
    }

    public virtual void AnimationStartMovementTrigger()
    {
        attackState.SetPlayerVelocity(weaponData.movementSpeed[attackCount]);
    }

    public virtual void AnimationStopMovementTrigger()
    {
        attackState.SetPlayerVelocity(0f);
    }

    public virtual void AnimationCheckOffFlip()
    {
        attackState.SetCheckFlip(false);
    }

    public virtual void AnimationActionTrigger()
    {
    }

    public virtual void AnimationCheckOnFlip()
    {
        attackState.SetCheckFlip(true);
    }

    #endregion

    public void InitializeWeapon(PlayerAttackState state, Core core)
    {
        this.attackState = state;
        this.core = core;
    }
}