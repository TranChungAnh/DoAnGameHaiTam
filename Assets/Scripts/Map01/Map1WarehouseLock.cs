using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Map1-only lock: first interaction shows key UI; after first unlock,
/// each interaction enters the warehouse immediately without key UI.
/// Replaces ONLY our earlier Map1WarehouseLock.cs, not shared game classes.
/// </summary>
public class Map1WarehouseLock : MonoBehaviour, IInteractable
{
    [Header("Player / tuong tac")]
    [SerializeField] private Transform player;
    [SerializeField, Min(0.1f)] private float interactionDistance = 1.5f;
    [SerializeField] private bool allowFKey = true;

    [Header("Tai nguyen")]
    [SerializeField] private ItemBase requiredKey;
    [SerializeField] private Map1WarehouseLockUI lockUI;
    [SerializeField] private Map1WarehouseTravel warehouseTravel;

    [Header("Hieu ung o khoa")]
    [SerializeField] private SpriteRenderer glowingOverlay;

    [Header("Trang thai cua - tuy chon")]
    [SerializeField] private Collider2D doorBlocker;
    [SerializeField] private GameObject closedDoorVisual;
    [SerializeField] private GameObject openedDoorVisual;

    private bool opened;
    private bool wasNear;
    private Color glowColor;

    public bool IsOpened => opened;
    public Sprite KeyIcon => requiredKey != null ? requiredKey.icon : null;

    private void Awake()
    {
        if (glowingOverlay != null)
        {
            glowColor = glowingOverlay.color;
            glowingOverlay.enabled = false;
        }
        if (openedDoorVisual != null) openedDoorVisual.SetActive(false);
    }

    private void Update()
    {
        bool near = IsPlayerInRange();

        if (glowingOverlay != null)
        {
            glowingOverlay.enabled = near;
            if (near)
            {
                Color color = glowColor;
                color.a = Mathf.Lerp(0.25f, 0.85f, Mathf.PingPong(Time.time * 1.5f, 1f));
                glowingOverlay.color = color;
            }
        }

        if (wasNear && !near && lockUI != null) lockUI.CloseIfCurrent(this);
        wasNear = near;

        if (near && allowFKey && Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void FindPlayerIfMissing()
    {
        if (player != null) return;
        PlayerController controller = FindObjectOfType<PlayerController>();
        if (controller != null) player = controller.transform;
    }

    public bool IsPlayerInRange()
    {
        FindPlayerIfMissing();
        if (player == null) return false;
        return ((Vector2)(player.position - transform.position)).sqrMagnitude
               <= interactionDistance * interactionDistance;
    }

    public bool HasKey()
    {
        return requiredKey != null &&
               !string.IsNullOrEmpty(requiredKey.itemName) &&
               InventoryManager.Instance != null &&
               InventoryManager.Instance.HasItem(requiredKey.itemName, 1);
    }

    public void Interact()
    {
        if (!IsPlayerInRange()) return;
        if (warehouseTravel == null || warehouseTravel.IsTransitioning || warehouseTravel.IsInside)
        {
            if (warehouseTravel == null)
                Debug.LogWarning("[Map1] Chua gan Warehouse Travel vao o khoa.", this);
            return;
        }

        if (opened)
        {
            warehouseTravel.EnterWarehouse();
            return;
        }

        if (requiredKey == null || lockUI == null)
        {
            Debug.LogWarning("[Map1] Chua gan Key Item hoac Lock UI.", this);
            return;
        }
        lockUI.Show(this);
    }

    // Called by the UNCHANGED Map1WarehouseLockUI when the user presses MO.
    public bool TryUnlock()
    {
        if (opened || !IsPlayerInRange() || !HasKey()) return false;
        if (warehouseTravel == null || !warehouseTravel.CanEnterWarehouse)
        {
            Debug.LogError("[Map1] Chua gan Travel hoac Scene nha kho chua co trong Build Settings.", this);
            return false;
        }
        if (warehouseTravel.IsTransitioning || warehouseTravel.IsInside) return false;

        // Start travel first; don't mark unlocked if it cannot start.
        if (!warehouseTravel.EnterWarehouse()) return false;
        opened = true;

        if (doorBlocker != null) doorBlocker.enabled = false;
        if (closedDoorVisual != null) closedDoorVisual.SetActive(false);
        if (openedDoorVisual != null) openedDoorVisual.SetActive(true);
        if (glowingOverlay != null) glowingOverlay.enabled = false;

        Debug.Log("[Map1] Da mo khoa; lan sau nhan F se vao thang nha kho.", this);
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
