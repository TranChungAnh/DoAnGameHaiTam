using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="QuestData_",menuName ="Game/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Cơ bản")]
    public string questID;
    public string questName;
    [TextArea] public string description;
    public bool isManinQuest = false;

    [Header("Mục tiêu(có thể nhiều mục tiêu)")]
    public QuestObjective[] objectives;
    [Header("phần thưởng")]
    public int goldReward;
    public ItemBase[] itemRewards;
}
