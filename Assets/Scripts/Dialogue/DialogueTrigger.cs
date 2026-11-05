using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueData dialogueData;

    [Header("Cài đặt Chạy Hội Thoại")]
    [Tooltip("Bật: Tự động chạy hội thoại theo thời gian. Tắt: Player phải bấm Next để chuyển.")]
    [SerializeField] private bool autoPlay = false;

    [Tooltip("Thời gian chờ (giây) trước khi tự động chuyển sang câu tiếp theo (chỉ dùng khi bật autoPlay)")]
    [SerializeField] private float autoPlayDelay = 3f;

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
            Debug.LogWarning($"{gameObject.name}: Collider2D chưa bật Is Trigger!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"DialogueTrigger nhận: {other.name}", this);

        if (!triggerOnEnter)
        {
            Debug.Log("Bị chặn: chưa bật Trigger On Enter", this);
            return;
        }

        if (!IsPlayerLayer(other.gameObject))
        {
            Debug.Log($"Bị chặn: sai Layer của {other.name}", this);
            return;
        }

        if (triggerOnce && hasTriggered)
        {
            Debug.Log("Bị chặn: đã kích hoạt một lần", this);
            return;
        }

        Debug.Log("Đang gọi StartDialogue", this);
        StartDialogue();

        if (triggerOnce)
            hasTriggered = true;
    }

    private bool IsPlayerLayer(GameObject target)
    {
        return (playerLayer.value & (1 << target.layer)) != 0;
    }

    public void StartDialogue()
    {
        if (dialogueData == null)
        {
            Debug.LogWarning($"DialogueTrigger '{gameObject.name}' chưa có DialogueData!");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy DialogueManager trong Scene!");
            return;
        }

        // Truyền thêm thông tin autoPlay và autoPlayDelay sang Manager
        DialogueManager.Instance.StartDialogue(dialogueData, autoPlay, autoPlayDelay);
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