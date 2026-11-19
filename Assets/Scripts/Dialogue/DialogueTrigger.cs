using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Trạng thái thế giới (Chống phát lại khi Load game)")]
    [Tooltip("Bật: Ghi nhớ trạng thái vĩnh viễn (không phát lại khi load game). Tắt: Reset mỗi khi chơi lại.")]
    [SerializeField] private bool rememberAcrossLoads = true;

    [Tooltip("Mã ID riêng cho từng hội thoại.")]
    public string uniqueDialogueID;

    private bool hasTriggered;
    private Collider2D triggerCollider;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueDialogueID))
        {
            uniqueDialogueID = gameObject.name + "_" +
                System.Guid.NewGuid().ToString().Substring(0, 8);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();

        if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning($"{gameObject.name}: Collider2D chưa bật Is Trigger!");
        }
    }

    private void Start()
    {
        // Kiểm tra hội thoại đã được kích hoạt trước đó chưa
        if (rememberAcrossLoads && !string.IsNullOrEmpty(uniqueDialogueID))
        {
            if (PlayerPrefs.GetInt("DialogueState_" + uniqueDialogueID, 0) == 1)
            {
                hasTriggered = true;
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnEnter)
            return;

        if (!IsPlayerLayer(other.gameObject))
            return;

        // Đã kích hoạt rồi
        if (triggerOnce && hasTriggered)
            return;

        // Kiểm tra đã lưu trạng thái từ lần chơi trước chưa
        if (rememberAcrossLoads &&
            PlayerPrefs.GetInt("DialogueState_" + uniqueDialogueID, 0) == 1)
        {
            hasTriggered = true;
            gameObject.SetActive(false);
            return;
        }

        // Kiểm tra và bắt đầu hội thoại
        if (dialogueData == null || DialogueManager.Instance == null)
        {
            StartDialogue();
            return;
        }

        StartDialogue();

        if (triggerOnce)
        {
            hasTriggered = true;

            // Lưu trạng thái để không phát lại khi Load Game
            if (rememberAcrossLoads && !string.IsNullOrEmpty(uniqueDialogueID))
            {
                PlayerPrefs.SetInt("DialogueState_" + uniqueDialogueID, 1);
                PlayerPrefs.Save();
            }

            gameObject.SetActive(false);
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
            Debug.LogWarning($"DialogueTrigger '{gameObject.name}' chưa có DialogueData!");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy DialogueManager trong Scene!");
            return;
        }

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