using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonState : State
{
    protected D_SummonState summonStateData;
    protected Transform attackPosition;

    protected float lastSummonTime = -Mathf.Infinity;
    protected float summonStartTime;

    protected bool isSummonCreated;
    protected bool isSummonFinished;

    public SummonState(
        Entity entity,
        FiniteStateMachine stateMachine,
        string animBoolName,
        Transform attackPosition,
        D_SummonState summonStateData)
        : base(entity, stateMachine, animBoolName)
    {
        this.attackPosition = attackPosition;
        this.summonStateData = summonStateData;
    }

    public override void Enter()
    {
        base.Enter();

        isSummonCreated = false;
        isSummonFinished = false;

        summonStartTime = Time.time;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isSummonCreated)
        {
            CreateSummon();
        }

        // Chờ kết thúc State sau thời gian quy định
        if (isSummonCreated && Time.time >= summonStartTime + summonStateData.summonDuration)
        {
            isSummonFinished = true;
        }
    }

    protected virtual void CreateSummon()
    {
        if (summonStateData == null || summonStateData.tentaclePrefab == null)
        {
            isSummonCreated = true;
            return;
        }

        entity.StartCoroutine(SpawnTentaclesRoutine());

        lastSummonTime = Time.time;
        isSummonCreated = true;
    }

    private IEnumerator SpawnTentaclesRoutine()
    {
        Transform spawnPoint = attackPosition != null ? attackPosition : entity.transform;
        Vector3 startPos = spawnPoint.position;

        // 1. Lấy hướng nhìn Top-Down từ Entity (mặc định xuống dưới nếu bằng Vector2.zero)
        Vector2 facingDirection = entity.LastFacingDirection;
        if (facingDirection == Vector2.zero)
        {
            facingDirection = Vector2.down;
        }

        // 2. Mọc súc tua đâm thẳng về phía trước theo hướng nhìn
        for (int i = 0; i < summonStateData.numberOfTentacles; i++)
        {
            // Tính vị trí súc tua tiếp theo theo vector hướng nhìn
            Vector3 spawnPos = startPos + (Vector3)(facingDirection * (i + 1) * summonStateData.spacing);

            // Thêm offset ngẫu nhiên vuông góc với hướng nhìn nếu bật randomOffset
            if (summonStateData.randomOffset)
            {
                Vector2 perpendicularDir = new Vector2(-facingDirection.y, facingDirection.x);
                float randomVal = Random.Range(-summonStateData.offsetRange, summonStateData.offsetRange);
                spawnPos += (Vector3)(perpendicularDir * randomVal);
            }

            // Tạo Súc tua
            Object.Instantiate(summonStateData.tentaclePrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(summonStateData.spawnDelay);
        }
    }
    public bool CanUseSummon()
    {
        return Time.time >= lastSummonTime + summonStateData.summonCooldown;
    }
}