using System;
using UnityEngine;

public enum QuestObjectiveType { Collect, Deliver, Talk, Harvest, Produce }

[Serializable]
public class QuestObjective
{
    public QuestObjectiveType type;
    public string targetID;
    public int requiredAmount = 1;

    [NonSerialized] public int currentAmount = 0;
    public bool isCompleted => currentAmount >= requiredAmount;

    public void ResetRuntime() => currentAmount = 0;

    public void AddProgress(int amount)
        => currentAmount = Mathf.Clamp(currentAmount + amount, 0, requiredAmount);

    public QuestObjective clone()
    {
        return new QuestObjective
        {
            type = type,
            targetID = targetID,
            requiredAmount = requiredAmount,
            currentAmount = 0
        };
    }
}
