
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class Map01WellNotePickup : MonoBehaviour, IInteractable
{
    [Header("C1-S06 - Mau ghi chu")]
    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 1.4f;

    [Header("Dialogue")]
    [SerializeField] private DialogueData noteDialogue;
    [SerializeField] private float autoNextSeconds = 10f;

    [Header("Inventory")]
    [SerializeField] private ItemBase noteItem;

    private Collider2D noteCollider;
    private bool isReading = false;
    private bool isCollected = false;

    private void Awake()
    {
        noteCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isReading || isCollected)
            return;

        if (!IsPlayerNear())
            return;

        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsTalking)
            return;

        // Nhan F de doc
        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            Interact();
            return;
        }

        // Chuot phai truc tiep vao ghi chu
        if (Mouse.current != null &&
            Mouse.current.rightButton.wasReleasedThisFrame)
        {
            if (!IsPointerOverUI() && RightClickHitsNote())
                Interact();
        }
    }

    public void Interact()
    {
        if (isReading || isCollected || !IsPlayerNear())
            return;

        DialogueManager manager = DialogueManager.Instance;

        if (manager == null || manager.IsTalking)
            return;

        if (noteDialogue == null ||
            noteDialogue.lines == null ||
            noteDialogue.lines.Count == 0 ||
            noteItem == null)
        {
            Debug.LogWarning(
                "[Map01 C1-S06] Chua gan Dialogue hoac Item."
            );
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning(
                "[Map01 C1-S06] Khong tim thay InventoryManager."
            );
            return;
        }

        manager.StartDialogue(
            noteDialogue,
            true,
            autoNextSeconds
        );

        if (!manager.IsTalking)
            return;

        isReading = true;
        StartCoroutine(WaitAndCollect(manager));
    }

    private IEnumerator WaitAndCollect(DialogueManager manager)
    {
        // Cho Dialogue bat dau
        yield return null;

        // Cho den khi tat ca cau thoai ket thuc
        while (manager != null && manager.IsTalking)
        {
            yield return null;
        }

        if (manager == null || InventoryManager.Instance == null)
        {
            isReading = false;
            yield break;
        }

        // Them vao Inventory
        bool success = InventoryManager.Instance.AddItem(
            noteItem, 1
        );

        if (!success)
        {
            Debug.LogWarning(
                "[Map01 C1-S06] Inventory day. " +
                "Ghi chu van o tren mat dat."
            );

            isReading = false;
            yield break;
        }

        isCollected = true;

        Debug.Log(
            "[Map01 C1-S06] Da thu thap mau ghi chu."
        );

        // Xoa ghi chu khoi ban do sau khi nhat
        gameObject.SetActive(false);
    }

    private bool IsPlayerNear()
    {
        if (player == null)
        {
            PlayerController controller =
                FindObjectOfType<PlayerController>();

            if (controller != null)
                player = controller.transform;
        }

        if (player == null)
            return false;

        return Vector2.Distance(
            player.position,
            transform.position
        ) <= interactionDistance;
    }

    private bool RightClickHitsNote()
    {
        if (Camera.main == null || Mouse.current == null)
            return false;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector2 worldPosition =
            Camera.main.ScreenToWorldPoint(mousePosition);

        Collider2D[] hits =
            Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D hit in hits)
        {
            if (hit == noteCollider ||
                hit.transform.IsChildOf(transform))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            interactionDistance
        );
    }
}
