using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newJumpAttackStateData", menuName = "Data/State Data/JumpAttack State")]

public class D_jumpAttackState : ScriptableObject
{
    public GameObject spikePrefab;
    public GameObject spawnPoint;
    public int numberOfSpikes=7;
    public float spikeSpacing= 1.5f;
    public float spikeLifeTime=1f;
    public float spawnDelay=0.1f;
    public bool randomOffset= false;
    public float offsetRange=0.5f;

}
