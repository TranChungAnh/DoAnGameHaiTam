using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSchedule : MonoBehaviour
{
    public List<NPCSchedulePoint> schedules;
    public NPCSchedulePoint GetSchedule(int hour,int minute)
    {
        foreach (var s in schedules)   
            if (s.hour == hour && s.minute == minute)
                return s;
        return null;
    }
    public Vector3 GetRandomPoint(NPCSchedulePoint s)
    {
        if(s.points==null || s.points.Count==0)
            return transform.position;

        int index = UnityEngine.Random.Range(0, s.points.Count);
        return s.points[index].position;
    }
}
