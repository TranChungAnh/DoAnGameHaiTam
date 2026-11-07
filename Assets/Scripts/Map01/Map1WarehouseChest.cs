using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Map 1 chest. F (near) or right-click (on chest) opens its own UI.
/// First unlocking requires Map1_ChestKey; contents are clicked to collect.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Map1WarehouseChest : MonoBehaviour, IInteractable
{
    [Header("Tuong tac")]
    [SerializeField] private Transform player;
    [SerializeField, Min(0.1f)] private float interactionDistance = 1.4f;

    [Header("Vat pham")]
    [SerializeField] private ItemBase requiredChestKey;
    [SerializeField] private ItemBase artifact01;

    [Header("Giao dien")]
    [SerializeField] private Map1WarehouseChestUI chestUI;

    [Header("Hinh anh")]
    [SerializeField] private GameObject closedChestVisual;
    [SerializeField] private GameObject openedChestVisual;
    [SerializeField] private SpriteRenderer chestGlow;
    [SerializeField, Min(0.1f)] private float glowSpeed = 1.6f;

    private Collider2D ownCollider;
    private Color originalGlowColor;

    public Sprite KeyIcon => requiredChestKey != null ? requiredChestKey.icon : null;
    public ItemBase Artifact => artifact01;
    public bool IsOpened => Map1WarehouseQuestState.Instance != null &&
                             Map1WarehouseQuestState.Instance.ChestOpened;
    public bool IsEmpty => Map1WarehouseQuestState.Instance != null &&
                            Map1WarehouseQuestState.Instance.Artifact01Collected;

    private void Awake()
    {
        ownCollider = GetComponent<Collider2D>();
        if (chestGlow != null)
        {
            chestGlow.gameObject.SetActive(true);
            originalGlowColor = chestGlow.color;
            chestGlow.enabled = false;
        }
    }

    private void Start()
    {
        FindChestUIIfNeeded();
        RefreshChestAppearance();
    }

    // The interior scene is loaded Additively and the UI Canvas lives in Map01.
    // Inspector references across two different scene assets cannot be saved.
    private void FindChestUIIfNeeded()
    {
        if (chestUI == null)
            chestUI = FindObjectOfType<Map1WarehouseChestUI>();
    }

    private void OnDisable()
    {
        FindChestUIIfNeeded();
        if (chestUI != null) chestUI.CloseIfCurrent(this);
    }

    private void Update()
    {
        FindChestUIIfNeeded();
        bool talking = DialogueManager.Instance != null && DialogueManager.Instance.IsTalking;
        bool uiOpen = chestUI != null && chestUI.IsOpen;
        bool nearby = !talking && !uiOpen && IsPlayerNear();

        if (chestGlow != null)
        {
            chestGlow.enabled = nearby;
            if (nearby)
            {
                Color c = originalGlowColor;
                c.a = Mathf.Lerp(0.25f, 0.85f,
                    Mathf.PingPong(Time.time * glowSpeed, 1f));
                chestGlow.color = c;
            }
        }

        if (!nearby) return;
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            Interact();

        if (Mouse.current != null && Mouse.current.rightButton.wasReleasedThisFrame &&
            !IsPointerOverUI() && RightClickHitsChest())
            Interact();
    }

    public void Interact()
    {
        FindChestUIIfNeeded();
        if (!IsPlayerNear() || chestUI == null || chestUI.IsOpen) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking) return;
        if (Map1WarehouseQuestState.Instance == null)
        {
            Debug.LogWarning("[Map1] Missing Map1WarehouseQuestState in Map01.", this);
            return;
        }
        chestUI.Show(this);
    }

    public bool HasKey()
    {
        return requiredChestKey != null &&
            InventoryManager.Instance != null &&
            InventoryManager.Instance.HasItem(requiredChestKey.itemName, 1);
    }

    public bool TryUnlock()
    {
        var state = Map1WarehouseQuestState.Instance;
        if (state == null || !IsPlayerNear()) return false;
        if (state.ChestOpened) return true;
        if (!HasKey()) return false;

        state.MarkChestOpened();
        RefreshChestAppearance();
        Debug.Log("[Map1] Chest opened.", this);
        return true;
    }

    public bool TryTakeArtifact()
    {
        var state = Map1WarehouseQuestState.Instance;
        if (state == null || !state.ChestOpened || state.Artifact01Collected ||
            !IsPlayerNear() || artifact01 == null || InventoryManager.Instance == null)
            return false;

        if (!InventoryManager.Instance.AddItem(artifact01, 1))
        {
            Debug.LogWarning("[Map1] Inventory full; artifact stays in chest.", this);
            return false;
        }

        state.MarkArtifact01Collected();
        UIInventory inventoryUI = FindObjectOfType<UIInventory>();
        if (inventoryUI != null && inventoryUI.gameObject.activeInHierarchy)
            inventoryUI.Refresh();
        Debug.Log("[Map1] Artifact 01 collected.", this);
        return true;
    }

    private void RefreshChestAppearance()
    {
        bool opened = IsOpened;
        if (closedChestVisual != null) closedChestVisual.SetActive(!opened);
        if (openedChestVisual != null) openedChestVisual.SetActive(opened);

        if (chestGlow != null)
        {
            SpriteRenderer target = null;
            if (opened && openedChestVisual != null)
                target = openedChestVisual.GetComponent<SpriteRenderer>();
            if (!opened && closedChestVisual != null)
                target = closedChestVisual.GetComponent<SpriteRenderer>();
            if (target != null) chestGlow.sprite = target.sprite;
        }
    }

    private bool IsPlayerNear()
    {
        if (player == null)
        {
            PlayerController controller = FindObjectOfType<PlayerController>();
            if (controller != null) player = controller.transform;
        }
        return player != null &&
            ((Vector2)(player.position - transform.position)).sqrMagnitude <=
            interactionDistance * interactionDistance;
    }

    private bool RightClickHitsChest()
    {
        if (Camera.main == null || Mouse.current == null) return false;
        Vector2 screen = Mouse.current.position.ReadValue();
        Vector2 point = Camera.main.ScreenToWorldPoint(screen);
        foreach (Collider2D hit in Physics2D.OverlapPointAll(point))
        {
            if (hit == ownCollider || hit.transform.IsChildOf(transform)) return true;
        }
        return false;
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
