using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<QuestInstance> activeQuests = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void AcceptQuest(QuestData data)
    {
        if (activeQuests.Exists(q => q.questID == data.questID))
            return;
        QuestInstance quest = new QuestInstance
        {
            questID = data.questID,
            questName = data.questName,
            description = data.description,
            goldReward = data.goldReward,
            itemRewardIDs = data.itemRewards,
            objectives = CloneObjectives(data.objectives)
        };
        activeQuests.Add(quest);
        QuestUI.Instance.Refresh();
    }
    private QuestObjective[] CloneObjectives(QuestObjective[] src)
    {
        QuestObjective[] arr=new QuestObjective[src.Length];
        for(int i = 0; i < src.Length; i++)
            arr[i] = src[i].clone();
            return arr;
        
    }
    public void AddProgress(QuestObjectiveType type, string targetID,int amount)
    {
        foreach (var quest in activeQuests)
            foreach (var obj in quest.objectives)
                if (obj.type == type && obj.targetID == targetID)
                    obj.AddProgress(amount);
        QuestUI.Instance.Refresh();

    }
    public void TryCompleteQuest(NPCQuestGiver npc)
    {
        foreach(var quest in activeQuests)
        {
            if (quest.isCompleted) continue;
            bool done = true;
            foreach (var obj in quest.objectives)
                if (!obj.isCompleted) done = false;
            if (!done) continue;

            CompleteQuest(quest);
            return;
        }
    }
    void CompleteQuest(QuestInstance quest)
    {
        quest.isCompleted = true;
        foreach(var id in quest.itemRewardIDs)
        {
            InventoryManager.Instance.AddItem(id);
        }

        Debug.Log(" Hoàn thành quest: " + quest.questName);
        QuestUI.Instance.Refresh();
    }
    }
