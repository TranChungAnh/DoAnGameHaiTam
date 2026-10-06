using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueData dialogueData;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnEnter = true;

    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();

        if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning(
                $"{gameObject.name}: Collider2D chưa bật Is Trigger!"
            );
        }
    }

  
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnEnter)
        {
            return;
        }

        // Kiểm tra Layer
        if (!IsPlayerLayer(other.gameObject))
        {
            return;
        }

        // Đã kích hoạt rồi
        if (triggerOnce && hasTriggered)
        {
            return;
        }

        StartDialogue();

        if (triggerOnce)
        {
            hasTriggered = true;
        }
    }

    private bool IsPlayerLayer(GameObject target)
    {
        return (playerLayer.value & (1 << target.layer)) != 0;
    }

    public void StartDialogue()
    {
        if (dialogueData == null)
        {
            Debug.LogWarning(
                $"DialogueTrigger '{gameObject.name}' chưa có DialogueData!"
            );

            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning(
                "Không tìm thấy DialogueManager trong Scene!"
            );

            return;
        }

        DialogueManager.Instance.StartDialogue(dialogueData);
    }

   
    public void ResetTrigger()
    {
        hasTriggered = false;
    }

 
    public void SetTriggered(bool value)
    {
        hasTriggered = value;
    }

    public bool HasTriggered()
    {
        return hasTriggered;
    }

  
    public void SetDialogue(DialogueData newDialogue)
    {
        dialogueData = newDialogue;
    }

    public DialogueData GetDialogue()
    {
        return dialogueData;
    }
}