using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newRangeState", menuName = "Data/State Data/Range Attack State")]

public class D_RangeAttackState : ScriptableObject
{
    public GameObject projectile;
    public float projecTileSpeed = 12f;
    public float projectTitleDamage = 10f;
    public float projectTitleTraveDistance;
}
