using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newStunData", menuName = "Data/State Data/Stun Data")]

public class D_StunState : ScriptableObject
{
    public float stunTime = 3f;
    public float stunKnockBackTime = 0.2f;
    public float stunKonckBackSpeed = 20f;

    public Vector2 stunKnockBackAngle;// góc đẩy 
}
