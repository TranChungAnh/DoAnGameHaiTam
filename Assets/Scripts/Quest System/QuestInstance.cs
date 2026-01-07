using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
public class QuestInstance : MonoBehaviour
{
    public string questID;
    public string questName;
    public string description;
    public QuestObjective[] objectives;

    public bool isActive = true;
    public bool isCompleted = false;

    public int goldReward;
    public ItemBase[] itemRewardIDs;
  

}
