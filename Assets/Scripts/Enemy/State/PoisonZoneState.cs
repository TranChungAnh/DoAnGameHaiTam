using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonZoneState : State
{
    protected GameObject poisonZonePrefab;

    protected float poisonZoneDuration;
    protected float poisonZoneCooldown;

    protected float lastPoisonZoneTime = -Mathf.Infinity;
    protected float poisonZoneStartTime;

    protected bool isPoisonZoneCreated;
    protected bool isPoisonZoneFinished;

    [Header("Poison Damage")]
    protected float poisonDamage = 5f;
    protected float damageInterval = 1f;
    protected float damageTimer;

    [Header("Poison Range")]
    protected float poisonZoneRadius = 1.5f;

    protected LayerMask whatIsPlayer;

    protected Vector3 damagePosition;

    public PoisonZoneState(
        Entity entity,
        FiniteStateMachine stateMachine,
        string animBoolName,
        GameObject poisonZonePrefab,
        float poisonZoneDuration,
        float poisonZoneCooldown,
        LayerMask whatIsPlayer)
        : base(entity, stateMachine, animBoolName)
    {
        this.poisonZonePrefab = poisonZonePrefab;
        this.poisonZoneDuration = poisonZoneDuration;
        this.poisonZoneCooldown = poisonZoneCooldown;
        this.whatIsPlayer = whatIsPlayer;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        isPoisonZoneCreated = false;
        isPoisonZoneFinished = false;

        poisonZoneStartTime = Time.time;
        damageTimer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isPoisonZoneCreated)
        {
            CreatePoisonZone();
        }

        if (isPoisonZoneCreated)
        {
            CheckPoisonDamage();
        }

        if (isPoisonZoneCreated &&
            Time.time >= poisonZoneStartTime + poisonZoneDuration)
        {
            isPoisonZoneFinished = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    protected virtual void CreatePoisonZone()
    {
        if (poisonZonePrefab == null)
        {
            isPoisonZoneCreated = true;
            return;
        }

        if (entity.Target == null)
        {
            isPoisonZoneCreated = true;
            return;
        }

        damagePosition = entity.Target.position;

        GameObject poisonZone = Object.Instantiate(
            poisonZonePrefab,
            damagePosition,
            Quaternion.identity
        );

        Object.Destroy(poisonZone, poisonZoneDuration);

        lastPoisonZoneTime = Time.time;
        isPoisonZoneCreated = true;

    }

    protected virtual void CheckPoisonDamage()
    {
        damageTimer += Time.deltaTime;

        if (damageTimer < damageInterval)
        {
            return;
        }

        Collider2D[] damageHit = Physics2D.OverlapCircleAll(
            damagePosition,
            poisonZoneRadius,
            whatIsPlayer
        );


        foreach (Collider2D collider in damageHit)
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.Damage(poisonDamage);

              
                if (States.Instance != null)
                {
                    float currentHealth = States.Instance.currentHealth;

                    if (currentHealth > 0)
                    {
                        States.Instance.setHealth(
                            currentHealth - poisonDamage
                        );

                        Debug.Log(
                            "Player Health: " +
                            (currentHealth - poisonDamage)
                        );
                    }
                }
                else
                {
                    Debug.LogError("States.Instance is null!");
                }
            }
        }

        damageTimer = 0f;
    }

    public bool CanUsePoisonZone()
    {
        return Time.time >= lastPoisonZoneTime + poisonZoneCooldown;
    }

    protected void DrawPoisonGizmo()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            damagePosition,
            poisonZoneRadius
        );
    }
}