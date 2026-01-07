using GameEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NPCSchedulePoint 
{
    public int hour;
    public int minute;
    public string placeName;
    public List<Transform> points;
    public NPCStateType stateType;

}
