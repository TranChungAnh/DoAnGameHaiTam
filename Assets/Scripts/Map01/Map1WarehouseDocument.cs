using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Walk close, press F: read the existing DialogueData.
/// After the player closes the dialogue, put the document into inventory,
/// hide its visual, and reveal the key underneath.
/// </summary>
public class Map1WarehouseDocument : MonoBehaviour, IInteractable
{
    [Header("Tuong tac")]
    [SerializeField, Min(0.1f)] private float interactionDistance = 1.35f;
    [SerializeField] private Transform player;

    [Header("Tai lieu")]
    [SerializeField] private DialogueData documentDialogue;
    [SerializeField] private ItemBase documentItem;
    [SerializeField] private GameObject documentVisual;
    [SerializeField] private SpriteRenderer documentGlow;

    [Header("Chia khoa nam duoi tai lieu")]
    [SerializeField] private GameObject chestKeyObject;

    [Header("Hieu ung")]
    [SerializeField, Min(0.1f)] private float glowSpeed = 1.6f;

    private bool reading;
    private Color baseGlowColor;

    private void Awake()
    {
        if (documentGlow != null)
        {
            baseGlowColor = documentGlow.color;
            documentGlow.enabled = false;
        }
    }

    private void Start()
    {
        RefreshVisuals();
    }

    private void Update()
    {
        Map1WarehouseQuestState state = Map1WarehouseQuestState.Instance;
        bool collected = state != null && state.DocumentCollected;
        bool talking = DialogueManager.Instance != null && DialogueManager.Instance.IsTalking;
        bool near = !collected && !reading && !talking && IsPlayerNear();

        if (documentGlow != null)
        {
            documentGlow.enabled = near;
            if (near)
            {
                Color c = baseGlowColor;
                c.a = Mathf.Lerp(0.25f, 0.85f, Mathf.PingPong(Time.time * glowSpeed, 1f));
                documentGlow.color = c;
            }
        }

        if (near && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            Interact();
    }

    public void Interact()
    {
        Map1WarehouseQuestState state = Map1WarehouseQuestState.Instance;
        if (state == null || state.DocumentCollected || reading || !IsPlayerNear()) return;
        if (DialogueManager.Instance == null || DialogueManager.Instance.IsTalking) return;

        if (documentDialogue == null || documentItem == null)
        {
            Debug.LogWarning("[Map1] DocumentDialogue/DocumentItem has not been assigned.", this);
            return;
        }

        if (documentDialogue.lines == null || documentDialogue.lines.Count == 0)
        {
            Debug.LogWarning("[Map1] Document dialogue has no lines.", this);
            return;
        }

        StartCoroutine(ReadThenCollect());
    }

    private IEnumerator ReadThenCollect()
    {
        reading = true;
        if (documentGlow != null) documentGlow.enabled = false;

        DialogueManager dialogueManager = DialogueManager.Instance;
        dialogueManager.StartDialogue(documentDialogue);

        // Wait until the last dialogue line is closed via its existing Next button.
        while (dialogueManager != null && dialogueManager.IsTalking)
            yield return null;

        if (InventoryManager.Instance == null || !InventoryManager.Instance.AddItem(documentItem, 1))
        {
            Debug.LogWarning("[Map1] Unable to collect document: inventory missing or full.", this);
            reading = false;
            yield break;
        }

        Map1WarehouseQuestState state = Map1WarehouseQuestState.Instance;
        if (state != null)
            state.MarkDocumentCollected();

        RefreshVisuals();
        RefreshInventoryUI();
        reading = false;
        Debug.Log("[Map1] Document collected; chest key revealed.", this);
    }

    private void RefreshVisuals()
    {
        Map1WarehouseQuestState state = Map1WarehouseQuestState.Instance;
        if (state == null)
        {
            Debug.LogWarning("[Map1] Add Map1WarehouseQuestState to the always-active Map01 travel object.", this);
            return;
        }

        if (documentVisual != null)
            documentVisual.SetActive(!state.DocumentCollected);

        if (documentGlow != null)
            documentGlow.enabled = false;

        if (chestKeyObject != null)
            chestKeyObject.SetActive(state.DocumentCollected && !state.ChestKeyCollected);
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

    private void RefreshInventoryUI()
    {
        UIInventory ui = FindObjectOfType<UIInventory>();
        if (ui != null && ui.gameObject.activeInHierarchy)
            ui.Refresh();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
