using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newEnityData",menuName ="Data/Enity Data/Base Data")]
public class D_Entity : ScriptableObject
{
    public float maxHealth = 30f;
    public float damageHopSpeed = 10f;
    public float wallCheckDistance = 0.2f;
    public LayerMask whatIsGround;
    public float minAgroDistance = 3f;
    public float maxAgroDistance = 4f;
    public float stunResistance = 3f;// khả năng kháng choáng 
    public float stunRecoveryTime = 2f;// thời gian choáng 
    public LayerMask whatIsPlayer;
    public float closeRangeActionDistance = 1f;
    public GameObject hitParticles;
}
