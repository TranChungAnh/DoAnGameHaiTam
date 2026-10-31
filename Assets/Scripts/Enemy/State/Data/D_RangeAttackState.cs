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
    [Header("Multiple Projectiles")]
    [Tooltip("Bật nếu muốn bắn nhiều quả đạn tỏa ra xung quanh")]
    public bool isMultipleProjectiles = false;
    [Tooltip("Số lượng quả đạn khi bắn tỏa tròn")]
    public int numberOfProjectiles = 8;
}
