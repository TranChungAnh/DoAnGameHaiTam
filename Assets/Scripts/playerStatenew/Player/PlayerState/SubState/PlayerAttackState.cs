using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    private Weapon weapon;
    private float velocityToSet;
    private bool setVelocity;

    private Vector2 rawInput;
    private Vector2 attackDirection;
    private bool isCheckFlip;

    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 1. Âm thanh tấn công
        if (SoundManager.Instance != null && SoundManager.Instance.swordSound != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.swordSound);
        }

        // 2. Kích hoạt logic Weapon
        if (weapon != null)
        {
            weapon.EnterWeapon();
        }
        else
        {
            Debug.LogError("PlayerAttackState: Weapon đang NULL!");
        }
        setVelocity = false;

        // 3. Lấy hướng bấm nút di chuyển của người chơi
        if (player != null && player.inputHandler != null)
        {
            rawInput = player.inputHandler.rawMovementInput;
        }

        // 4. XỬ LÝ HƯỚNG TẤN CÔNG CHUẨN TOP-DOWN
        if (rawInput != Vector2.zero)
        {
            attackDirection = rawInput.normalized;
        }
        else
        {
            // Ưu tiên lấy Vector2 FacingDirection từ Movement
            if (Movement != null && Movement.FacingDirection != Vector2.zero)
            {
                attackDirection = Movement.FacingDirection;
            }
            else
            {
                // Hướng mặc định khi mới vào game chưa bấm gì (thường Top-down là hướng Xuống)
                attackDirection = Vector2.down;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        if (weapon != null)
        {
            weapon.ExitWeapon();
            Debug.LogError("PlayerAttackState: Weapon chạy ddddddddddddddd!");

        }
        else
        {
            Debug.LogError("PlayerAttackState: Weapon đang NULL khi Exit!");
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player != null && player.inputHandler != null)
        {
            rawInput = player.inputHandler.rawMovementInput;
        }

        if (setVelocity)
        {
            Movement?.SetVelocity(attackDirection * velocityToSet);
        }
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();
        isAbilityDone = true;
    }

    // Gán Weapon từ bên ngoài vào State
    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
        if (this.weapon != null)
        {
            this.weapon.InitializeWeapon(this, core);
        }
    }

    public void SetPlayerVelocity(float velocity)
    {
        velocityToSet = velocity;
        Movement?.SetVelocity(attackDirection * velocityToSet);
        setVelocity = true;
    }

    public void SetCheckFlip(bool flip)
    {
        isCheckFlip = flip;
    }
}