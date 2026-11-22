using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class StorageCabinet : MonoBehaviour
{
    [Header("GameObject Trạng Thái (Ẩn/Hiện)")]
    [SerializeField] private GameObject closedGameObject;
    [SerializeField] private GameObject openGameObject;

    [Header("Nội dung trong tủ (chỉ hiện khi tủ mở)")]
    [SerializeField] private GameObject[] contents;

    [Header("Tương tác")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool canClose = true; // Sửa từ false thành true ở đây
    [SerializeField] private GameObject promptUI;

    [Header("Vùng nhận tương tác (khung vàng trong Scene)")]
    [SerializeField] private Vector2 interactSize = new Vector2(11f, 9.5f);
    [SerializeField] private Vector2 interactOffset = Vector2.zero;

   [SerializeField] private bool openAfterDialogue = true;

    [Header("Sự kiện")]
    public UnityEvent onOpened;
    public UnityEvent onClosed;

    private bool playerInRange;
    private bool isOpen;

    private void Awake()
    {
        SetState(false, false);
    }

    private void Update()
    {
        bool near = IsPlayerNear();
        if (near != playerInRange)
        {
            playerInRange = near;
            UpdatePrompt();
        }

        // Kiểm tra các điều kiện chặn
        if (!playerInRange) return;


        if (Keyboard.current == null || !Keyboard.current.fKey.wasPressedThisFrame) return;
        if (!isOpen) TryOpen();
        else if (canClose) SetState(false, true);
    }

    private void TryOpen()
    {
        SetState(true, true);
    }

   
    private bool IsPlayerNear()
    {
        Vector2 center = (Vector2)transform.position + interactOffset;
        return Physics2D.OverlapBox(center, interactSize, 0f, playerLayer) != null;
    }

    private void SetState(bool open, bool fireEvent)
    {
        isOpen = open;

        // Thay đổi ẩn/hiện GameObject theo trạng thái
        if (closedGameObject != null) closedGameObject.SetActive(!open);
        if (openGameObject != null) openGameObject.SetActive(open);

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
            promptUI.SetActive(playerInRange && (!isOpen || canClose));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + interactOffset, interactSize);
    }
}