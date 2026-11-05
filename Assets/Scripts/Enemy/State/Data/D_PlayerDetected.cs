using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="newDetectedState",menuName ="Data/State Data/PlayerDetected State")]
public class D_PlayerDetected : ScriptableObject
{
    public float longRangeActionTime = 1.5f;// độ chễ sau mỗi lần enemy tấn công player tầm xa 
}
