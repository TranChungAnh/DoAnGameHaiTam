using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newRoarStateData", menuName = "Data/State Data/Roar State")]
public class D_RoarState : ScriptableObject
{
    public float roarRadius = 5f;         // Bán kính sóng âm lan tỏa hình tròn
    public float stunDuration = 2f;       // Thời gian Player bị stun
    public float roarCooldown = 10f;      // Thời gian hồi chiêu kỹ năng Hét
    public LayerMask whatIsPlayer;        // Layer của Player để quét va chạm
}