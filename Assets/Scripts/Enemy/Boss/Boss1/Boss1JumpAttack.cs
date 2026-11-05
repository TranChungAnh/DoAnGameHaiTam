using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1JumpAttack : MonoBehaviour
{
    [Header("spike settings")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spacing = 1.5f;
    [SerializeField] private int numberOfSpikes = 7;
    [SerializeField] private float spikeLifeTime = 1f;
    [SerializeField] private float spawnDelay = 0.1f;

    [Header("Optional Effects")]
    [SerializeField] private bool randomOffset = false;       // Có nên tạo cây đinh lệch ngẫu nhiên
    [SerializeField] private float offsetRange = 0.5f;
    [SerializeField] private float attackInterval = 10f;
    [Header("Animation")]
    [SerializeField] private Animator bossAnimator;

    //private void Awake()
    //{
    //    StartCoroutine(JumpAttackLoop());
    //}

    //private IEnumerator JumpAttackLoop()
    //{
    //    while (true)
    //    {
    //        if (bossAnimator != null)
    //        {
    //            bossAnimator.SetBool("jumpAttack",true);
    //            Debug.LogError("Jump Attack Triggered");
    //            StartCoroutine(SpawnSpikes());
    //        }


    //        yield return new WaitForSeconds(attackInterval);
    //    }
    //}

    private void TriggertJumpAttack()
    {
        bossAnimator.SetBool("jumpAttack", false);

        StartCoroutine(SpawnSpikes());
    }
    private IEnumerator SpawnSpikes()
    {
        float centerX= spawnPoint.position.x;
        float startX = centerX - ((numberOfSpikes - 1) * spacing) / 2f;
        float y=spawnPoint.position.y;
        for(int i = 0; i < numberOfSpikes; i++)
        {
            float x = startX + i * spacing;
            if (randomOffset)
            {
                x+=Random.Range(-offsetRange, offsetRange);
            }
            Vector3 spawnpos = new Vector3(x, y, 0f);
            GameObject spike = Instantiate(spikePrefab, spawnpos, Quaternion.identity);
            Animator anim=spike.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Grow");
            }
            Destroy(spike, spikeLifeTime);

            yield return new WaitForSeconds(spawnDelay);
        }


    }
}
