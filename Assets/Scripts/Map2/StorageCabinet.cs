using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class StorageCabinet : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("Nội dung trong tủ (chỉ hiện khi tủ mở)")]
    [SerializeField] private GameObject[] contents;

    [Header("Tương tác")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Key interactKey = Key.E;
    [SerializeField] private bool canClose = false;
    [SerializeField] private GameObject promptUI;

    [Header("Vùng nhận tương tác (khung vàng trong Scene)")]
    [SerializeField] private Vector2 interactSize = new Vector2(11f, 9.5f);
    [SerializeField] private Vector2 interactOffset = Vector2.zero;

    [Header("Hội thoại ký ức (tùy chọn)")]
    [SerializeField] private DialogueTrigger memoryDialogue;
    [Tooltip("Bật: xem hội thoại xong mới mở tủ. Tắt: mở tủ rồi chạy hội thoại.")]
    [SerializeField] private bool openAfterDialogue = true;

    [Header("Debug")]
    [SerializeField] private bool debugLog = true;

    [Header("Sự kiện")]
    public UnityEvent onOpened;
    public UnityEvent onClosed;

    private SpriteRenderer sr;
    private bool playerInRange;
    private bool isOpen;
    private bool dialoguePlayed;
    private bool busy;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (playerLayer.value == 0)
            Debug.LogWarning("StorageCabinet: Player Layer đang để Nothing, hãy tick layer Player!");

        SetState(false, false);
    }

    private void Update()
    {
        bool near = IsPlayerNear();
        if (near != playerInRange)
        {
            playerInRange = near;
            UpdatePrompt();
            if (debugLog) Debug.Log(near ? "Tủ: người chơi đã ở gần" : "Tủ: người chơi đã rời xa");
        }

        if (!playerInRange || busy) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking) return;
        if (Keyboard.current == null) return;
        if (!Keyboard.current[interactKey].wasPressedThisFrame) return;

        if (debugLog) Debug.Log("Tủ: đã bấm phím tương tác");

        if (!isOpen) TryOpen();
        else if (canClose) SetState(false, true);
    }

    private void TryOpen()
    {
        if (memoryDialogue != null && !dialoguePlayed)
        {
            dialoguePlayed = true;
            StartCoroutine(OpenWithDialogue());
        }
        else
        {
            SetState(true, true);
        }
    }

    private IEnumerator OpenWithDialogue()
    {
        busy = true;

        if (openAfterDialogue)
        {
            memoryDialogue.StartDialogue();
            yield return null; // đợi Manager đặt IsTalking = true

            while (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking)
                yield return null;

            SetState(true, true);
        }
        else
        {
            SetState(true, true);
            memoryDialogue.StartDialogue();
        }

        busy = false;
    }

    private bool IsPlayerNear()
    {
        Vector2 center = (Vector2)transform.position + interactOffset;
        return Physics2D.OverlapBox(center, interactSize, 0f, playerLayer) != null;
    }

    private void SetState(bool open, bool fireEvent)
    {
        isOpen = open;
        sr.sprite = open ? openSprite : closedSprite;

        foreach (var go in contents)
            if (go != null) go.SetActive(open);

        if (fireEvent)
        {
            if (open) onOpened.Invoke();
            else onClosed.Invoke();
        }
        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(playerInRange && !busy && (!isOpen || canClose));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + interactOffset, interactSize);
    }
}