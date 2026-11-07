using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttackState : State
{
    protected D_jumpAttackState stateData;
    protected Boss1 boss; // Hoặc kiểu Boss phù hợp với enemy bạn dùng
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;
    public JumpAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_jumpAttackState stateData, Boss1 boss)
        : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();
        //boss.StartCoroutine(SpawnSpikes());
    }

    //private IEnumerator SpawnSpikes()
    //{
    //    Debug.LogError("Jump Attack Triggered2222222222222222222");

    //    float centerX = stateData.spawnPoint.transform.position.x;
    //    float startX = centerX - ((stateData.numberOfSpikes - 1) * stateData.spikeSpacing) / 2f;
    //    float y = stateData.spawnPoint.transform.position.y;
    //    for (int i = 0; i < stateData.numberOfSpikes; i++)
    //    {
    //        float x = startX + i * stateData.spikeSpacing;
    //        if (stateData.randomOffset)
    //        {
    //            x += Random.Range(-stateData.offsetRange, stateData.offsetRange);
    //        }
    //        Vector3 spawnpos = new Vector3(x, y, 0f);
    //        GameObject spike = GameObject.Instantiate(stateData.spikePrefab, spawnpos, Quaternion.identity);
    //        Animator anim = spike.GetComponent<Animator>();
    //        if (anim != null)
    //        {
    //            anim.SetTrigger("Grow");
    //        }
    //        GameObject.Destroy(spike, stateData.spikeLifeTime);

    //        yield return new WaitForSeconds(stateData.spawnDelay);
    //    }

    //}
}