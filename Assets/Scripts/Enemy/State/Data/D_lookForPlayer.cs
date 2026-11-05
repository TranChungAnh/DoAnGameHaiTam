using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newlookForPlayerStateData", menuName = "Data/State Data/Look For Player Data")]

public class D_lookForPlayer : ScriptableObject
{
    public int amountOfTurns = 2;//số lần xoay của một enemy 
    public float timeBetweenTurns = 0.75f;
}
