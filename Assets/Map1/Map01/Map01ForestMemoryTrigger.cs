
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Map01ForestMemoryTrigger : MonoBehaviour
{
    [Header("C1-S09 - Required Artifacts")]
    [SerializeField] private ItemBase artifact01;
    [SerializeField] private ItemBase artifact02;

    [Header("Dialogue")]
    [SerializeField] private DialogueData memoryDialogue;
    [SerializeField] private float autoNextSeconds = 10f;

    private bool activated = false;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryActivate(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryActivate(other);
    }

    private bool IsPlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return true;

        if (other.attachedRigidbody != null &&
            other.attachedRigidbody.CompareTag("Player"))
            return true;

        return other.GetComponentInParent<PlayerController>()
            != null;
    }

    private void TryActivate(Collider2D other)
    {
        if (activated || !IsPlayer(other))
            return;

        DialogueManager manager = DialogueManager.Instance;
        InventoryManager inventory = InventoryManager.Instance;

        if (manager == null ||
            manager.IsTalking ||
            inventory == null)
            return;

        if (artifact01 == null ||
            artifact02 == null ||
            memoryDialogue == null ||
            memoryDialogue.lines == null ||
            memoryDialogue.lines.Count == 0)
        {
            Debug.LogWarning(
                "[Map01 C1-S09] Chua gan du Item hoac Dialogue."
            );
            return;
        }

        bool has01 = inventory.HasItem(
            artifact01.itemName, 1
        );

        bool has02 = inventory.HasItem(
            artifact02.itemName, 1
        );

        // Chua du 2 co vat thi khong kich hoat
        if (!has01 || !has02)
            return;

        activated = true;

        Debug.Log(
            "[Map01 C1-S09] Du 2 co vat. Bat dau hoi thoai."
        );

        // Anh ky uc duoc hien truc tiep
        // thong qua Background cua DialogueData
        manager.StartDialogue(
            memoryDialogue,
            true,
            autoNextSeconds
        );
    }
}
