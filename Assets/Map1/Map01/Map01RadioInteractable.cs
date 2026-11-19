using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Map01RadioInteractable : MonoBehaviour, IInteractable
{
    [Header("C1-S04 - Radio cu cua Nguyen An")]
    // Đã loại bỏ biến player Transform vì chuyển sang dùng Trigger

    [Header("Dialogue")]
    [SerializeField] private DialogueData radioDialogue;
    [SerializeField] private float autoNextSeconds = 10f;

    //[Header("Inventory")]
    //[SerializeField] private ItemBase radioItem;

    [Header("Audio - Optional")]
    [SerializeField] private AudioSource radioAudio;
    [SerializeField] private AudioClip staticNoise;

    [Header("After Completion")]
    [SerializeField] private UnityEvent onCompleted;

    private bool started;
    private bool completed;
    private bool playerInRange = false; // Biến kiểm tra Player có trong vùng BoxCollider không

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (started || completed || !playerInRange)
            return;

        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsTalking)
            return;

        bool pressF = Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame;

        if (pressF)
            Interact();
    }

    public void Interact()
    {
        if (started || completed)
            return;

        var manager = DialogueManager.Instance;

        if (manager == null || manager.IsTalking)
            return;

        //if (radioDialogue == null || radioItem == null)
        //{
        //    Debug.LogWarning(
        //        "[Map01 C1-S04] Chua gan Dialogue hoac Item."
        //    );
        //    return;
        //}

        //if (InventoryManager.Instance == null)
        //{
        //    Debug.LogWarning(
        //        "[Map01 C1-S04] Khong tim thay InventoryManager."
        //    );
        //    return;
        //}

        started = true;

        if (radioAudio != null && staticNoise != null)
        {
            radioAudio.clip = staticNoise;
            radioAudio.loop = true;
            radioAudio.Play();
        }

        manager.StartDialogue(
            radioDialogue,
            true,
            autoNextSeconds
        );

        StartCoroutine(WaitForDialogueEnd());
    }

    private IEnumerator WaitForDialogueEnd()
    {
        yield return null;

        var manager = DialogueManager.Instance;

        while (manager != null && manager.IsTalking)
        {
            yield return null;
        }

        if (radioAudio != null)
            radioAudio.Stop();

        //InventoryManager.Instance.AddItem(radioItem, 1);

        completed = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        SpriteRenderer sprite =
            GetComponent<SpriteRenderer>();

        if (sprite != null)
            sprite.enabled = false;

        onCompleted?.Invoke();

        Debug.Log(
            "[Map01 C1-S04] Da nhan Radio cu."
        );
    }
}