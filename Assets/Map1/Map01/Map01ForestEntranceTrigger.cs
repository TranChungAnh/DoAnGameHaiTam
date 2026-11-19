
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class Map01ForestEntranceTrigger : MonoBehaviour
{
    [Header("C1-S09 - Dieu kien")]
    [SerializeField] private ItemBase artifact01;
    [SerializeField] private ItemBase artifact02;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Ky uc bat thuong")]
    [SerializeField] private GameObject memoryPanel;
    [SerializeField] private Image memoryImage;
    [SerializeField] private Sprite[] memorySprites;
    [SerializeField] private float imageDuration = 2.5f;

    [Header("Hoi thoai")]
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private float autoNextSeconds = 10f;

    [Header("Chuyen sang Map02")]
    [SerializeField] private UnityEvent onMap02Requested;

    private bool activated;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;

        if (memoryPanel != null)
            memoryPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStart(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryStart(other);
    }

    private bool IsPlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return true;

        return other.attachedRigidbody != null &&
            other.attachedRigidbody.CompareTag("Player");
    }

    private void TryStart(Collider2D other)
    {
        if (activated || !IsPlayer(other))
            return;

        DialogueManager manager = DialogueManager.Instance;

        if (manager == null || manager.IsTalking)
            return;

        if (dialogueData == null ||
            dialogueData.lines == null ||
            dialogueData.lines.Count == 0)
            return;

        if (artifact01 == null || artifact02 == null ||
            InventoryManager.Instance == null)
        {
            Debug.LogWarning(
                "[Map01 C1-S09] Chua gan day du du lieu."
            );
            return;
        }

        bool has01 = InventoryManager.Instance.HasItem(
            artifact01.itemName, 1
        );

        bool has02 = InventoryManager.Instance.HasItem(
            artifact02.itemName, 1
        );

        if (!has01 || !has02)
            return;

        activated = true;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        bool previousCanMove = true;

        if (player != null)
        {
            previousCanMove = player.canMove;
            player.canMove = false;
        }

        // Hien thi hai hinh anh ky uc
        if (memoryPanel != null &&
            memoryImage != null &&
            memorySprites != null &&
            memorySprites.Length > 0)
        {
            memoryPanel.SetActive(true);

            foreach (Sprite sprite in memorySprites)
            {
                if (sprite == null)
                    continue;

                memoryImage.sprite = sprite;

                yield return new WaitForSecondsRealtime(
                    imageDuration
                );
            }

            memoryPanel.SetActive(false);
        }

        // Hoi thoai cua Minh
        DialogueManager manager = DialogueManager.Instance;

        if (manager == null)
        {
            if (player != null)
                player.canMove = previousCanMove;

            activated = false;
            yield break;
        }

        manager.StartDialogue(
            dialogueData,
            true,
            autoNextSeconds
        );

        yield return null;

        while (manager != null && manager.IsTalking)
            yield return null;

        // Khoi phuc dieu khien truoc khi ban giao
        if (player != null)
            player.canMove = previousCanMove;

        // Chuyen truc tiep sang Map02
        if (onMap02Requested != null &&
            onMap02Requested.GetPersistentEventCount() > 0)
        {
            onMap02Requested.Invoke();
        }
        else
        {
            Debug.LogWarning(
                "[Map01 C1-S09] Chua ket noi chuyen Map02."
            );
        }
    }
}
