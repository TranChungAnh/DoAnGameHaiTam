using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCQuestGiver questGiver;
    private void Awake()
    {
        questGiver = GetComponent<NPCQuestGiver>();
    }
    public void Interact()
    {
        questGiver.OpenQuestPanel();
    }
}
