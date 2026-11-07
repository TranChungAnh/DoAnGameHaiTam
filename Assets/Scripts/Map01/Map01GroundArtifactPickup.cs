using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class Map01GroundArtifactPickup : MonoBehaviour, IInteractable
{
    [Header("C1-S03 - Hoi thoai sau khi nhat")]
    [SerializeField] private DialogueData pickupDialogue;

    [Header("Tuong tac")]
    [SerializeField] private Transform player;

    [SerializeField, Min(0.1f)]
    private float interactionDistance = 1.4f;

    [Header("Tuy chon them vao Inventory")]
    [SerializeField] private bool addToInventory = false;
    [SerializeField] private ItemBase artifactItem;


    [Header("Chan loi vao lang - C1-S03")]
    [SerializeField] private GameObject villageBarrier;


    private bool collected;
    private Collider2D artifactCollider;

    private void Awake()
    {
        artifactCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (collected)
            return;

        FindPlayerIfNeeded();

        if (!IsPlayerNear())
            return;

        // Khong cho nhat khi dang hoi thoai
        DialogueManager manager = DialogueManager.Instance;

        if (manager != null && manager.IsTalking)
            return;

        // Nhan F de nhat
        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            Interact();
            return;
        }

        // Chuot phai truc tiep vao co vat
        if (Mouse.current != null &&
            Mouse.current.rightButton.wasReleasedThisFrame &&
            !IsPointerOverUI() &&
            RightClickHitsArtifact())
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (collected || !IsPlayerNear())
            return;

        DialogueManager manager = DialogueManager.Instance;

        if (manager == null || manager.IsTalking ||
            pickupDialogue == null)
        {
            Debug.LogWarning(
                "[Map01 C1-S03] DialogueManager hoac DialogueData chua san sang.",
                this
            );
            return;
        }

        // Neu can luu co vat vao Inventory
        if (addToInventory)
        {
            if (artifactItem == null ||
                InventoryManager.Instance == null)
            {
                Debug.LogWarning(
                    "[Map01 C1-S03] Thieu ItemBase hoac InventoryManager.",
                    this
                );
                return;
            }

            bool added = InventoryManager.Instance.AddItem(
                artifactItem, 1
            );

            if (!added)
            {
                Debug.LogWarning(
                    "[Map01 C1-S03] Khong the nhat co vat: Inventory day.",
                    this
                );
                return;
            }

            UIInventory inventoryUI =
                FindObjectOfType<UIInventory>();

            if (inventoryUI != null &&
                inventoryUI.gameObject.activeInHierarchy)
            {
                inventoryUI.Refresh();
            }
        }


        // Danh dau da nhat thanh cong
        collected = true;

        // Mo loi vao lang sau khi nhat co vat
        if (villageBarrier != null)
        {
            villageBarrier.SetActive(false);

            Debug.Log(
                "[Map01 C1-S03] Village barrier unlocked.",
                this
            );
        }


        Debug.Log(
            "[Map01 C1-S03] Ground artifact collected.",
            this
        );

        // Bat dau hoi thoai sau khi nhat
        manager.StartDialogue(pickupDialogue, true, 10f);

        // An co vat tren mat dat
        gameObject.SetActive(false);
    }

    private void FindPlayerIfNeeded()
    {
        if (player != null)
            return;

        PlayerController controller =
            FindObjectOfType<PlayerController>();

        if (controller != null)
            player = controller.transform;
    }

    private bool IsPlayerNear()
    {
        if (player == null)
            return false;

        float distance =
            Vector2.Distance(player.position, transform.position);

        return distance <= interactionDistance;
    }

    private bool RightClickHitsArtifact()
    {
        if (Camera.main == null || Mouse.current == null)
            return false;

        Vector2 screenPosition =
            Mouse.current.position.ReadValue();

        Vector2 worldPosition =
            Camera.main.ScreenToWorldPoint(screenPosition);

        Collider2D[] hits =
            Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D hit in hits)
        {
            if (hit == artifactCollider ||
                hit.transform.IsChildOf(transform))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject();
    }
}
