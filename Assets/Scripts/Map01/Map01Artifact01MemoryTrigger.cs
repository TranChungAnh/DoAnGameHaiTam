
using System.Collections;
using UnityEngine;

public class Map01Artifact01MemoryTrigger : MonoBehaviour
{
    [Header("Ky uc Co Vat 01 - Map01")]
    [SerializeField] private DialogueData memoryDialogue;

    [SerializeField] private float autoPlayDelay = 3f;

    private bool hasPlayed;
    private bool isStarting;

    public void PlayAfterPickup()
    {
        if (hasPlayed || isStarting || memoryDialogue == null)
            return;

        StartCoroutine(StartMemory());
    }

    private IEnumerator StartMemory()
    {
        isStarting = true;

        // Cho giao dien ruong ket thuc xu ly
        yield return null;

        DialogueManager manager = DialogueManager.Instance;

        if (manager == null || manager.IsTalking)
        {
            Debug.LogWarning(
                "[Map01] DialogueManager chua san sang."
            );

            isStarting = false;
            yield break;
        }

        if (Time.timeScale <= 0f)
        {
            Debug.LogWarning(
                "[Map01] Time.timeScale dang bang 0."
            );

            isStarting = false;
            yield break;
        }

        manager.StartDialogue(
            memoryDialogue,
            true,
            autoPlayDelay
        );

        hasPlayed = manager.IsTalking;
        isStarting = false;
    }
}
