using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestOfferPanel : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI rewardText;
    private NPCQuestGiver currentNPC;
    private QuestData currentQuest;

    public void Show(NPCQuestGiver npc,QuestData quest)
    {
        currentNPC = npc;
        currentQuest = quest;

        titleText.text = quest.questName;
        descText.text = quest.description;
        rewardText.text = $"Gold: {quest.goldReward}";

        gameObject.SetActive(true);
    }
    public void AcceptQuest()
    {
        currentNPC.GiveQuest(currentQuest);
        gameObject.SetActive(false);
    }
    public void CloseQuest()
    {
        gameObject.SetActive(false);
    }
}
