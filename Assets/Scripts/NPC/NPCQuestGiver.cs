using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCQuestGiver : MonoBehaviour
{
    public QuestData quests;
    public QuestOfferPanel questPanel;
    public void OpenQuestPanel()
    {
        questPanel.Show(this, quests);
    }
    public void GiveQuest(QuestData data)
    {
        QuestManager.Instance.AcceptQuest(data);
    }
    public void TryCompleteQuest()
    {
        QuestManager.Instance.TryCompleteQuest(this);
    }
}
