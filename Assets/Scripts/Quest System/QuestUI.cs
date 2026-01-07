using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Refresh()
    {
        // TODO:
        // Clear list
        // Loop QuestManager.Instance.activeQuests
        // Hi?n questName + ti?n trình
        Debug.Log("?? Refresh Quest UI");
    }
}
