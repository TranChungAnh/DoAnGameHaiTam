using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Map 1 chest key: appears only after the document was collected.
/// F within range or right-click directly on its Collider2D collects it.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Map1WarehouseChestKey : MonoBehaviour, IInteractable
{
    [Header("Tuong tac")]
    [SerializeField, Min(0.1f)] private float interactionDistance = 1.35f;
    [SerializeField] private Transform player;

    [Header("Chia khoa")]
    [SerializeField] private ItemBase keyItem;
    [SerializeField] private SpriteRenderer keyGlow;
    [SerializeField, Min(0.1f)] private float glowSpeed = 1.6f;

    private Color originalGlowColor;
    private int activationFrame;
    private Collider2D keyCollider;

    private void Awake()
    {
        keyCollider = GetComponent<Collider2D>();
        if (keyGlow != null)
        {
            // The GameObject must be active; only enable/disable its renderer.
            keyGlow.gameObject.SetActive(true);
            originalGlowColor = keyGlow.color;
            keyGlow.enabled = false;
        }
    }

    private void OnEnable()
    {
        // Do not pick up on the F that ended the document dialogue.
        activationFrame = Time.frameCount;
    }

    private void Update()
    {
        var state = Map1WarehouseQuestState.Instance;
        if (state == null || !state.DocumentCollected || state.ChestKeyCollected)
        {
            if (keyGlow != null) keyGlow.enabled = false;
            return;
        }

        bool talking = DialogueManager.Instance != null && DialogueManager.Instance.IsTalking;
        bool nearby = !talking && IsPlayerNear();

        if (keyGlow != null)
        {
            keyGlow.enabled = nearby;
            if (nearby)
            {
                Color color = originalGlowColor;
                color.a = Mathf.Lerp(0.25f, 0.85f,
                    Mathf.PingPong(Time.time * glowSpeed, 1f));
                keyGlow.color = color;
            }
        }

        if (!nearby || Time.frameCount <= activationFrame) return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            Interact();

        // Fallback: the shared InteractionSystem uses only the FIRST raycast hit.
        // This checks OUR Collider2D even if the table's collider is also present.
        if (Mouse.current != null && Mouse.current.rightButton.wasReleasedThisFrame
            && !IsPointerOverUI() && RightClickHitsThisKey())
            Interact();
    }

    public void Interact()
    {
        var state = Map1WarehouseQuestState.Instance;
        if (state == null || !state.DocumentCollected || state.ChestKeyCollected ||
            Time.frameCount <= activationFrame || !IsPlayerNear()) return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking) return;
        if (keyItem == null || InventoryManager.Instance == null)
        {
            Debug.LogWarning("[Map1] Assign Key Item and ensure InventoryManager exists.", this);
            return;
        }
        if (!InventoryManager.Instance.AddItem(keyItem, 1))
        {
            Debug.LogWarning("[Map1] Inventory full; chest key not collected.", this);
            return;
        }

        state.MarkChestKeyCollected();
        UIInventory ui = FindObjectOfType<UIInventory>();
        if (ui != null && ui.gameObject.activeInHierarchy) ui.Refresh();
        Debug.Log("[Map1] Chest key collected.", this);
        gameObject.SetActive(false);
    }

    private bool IsPlayerNear()
    {
        if (player == null)
        {
            PlayerController controller = FindObjectOfType<PlayerController>();
            if (controller != null) player = controller.transform;
        }
        return player != null &&
            ((Vector2)(player.position - transform.position)).sqrMagnitude
            <= interactionDistance * interactionDistance;
    }

    private bool RightClickHitsThisKey()
    {
        if (Camera.main == null || Mouse.current == null) return false;
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 point = Camera.main.ScreenToWorldPoint(mouseScreen);
        Collider2D[] hits = Physics2D.OverlapPointAll(point);
        foreach (Collider2D hit in hits)
        {
            if (hit == keyCollider || hit.transform.IsChildOf(transform)) return true;
        }
        return false;
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
