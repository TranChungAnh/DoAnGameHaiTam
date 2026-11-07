
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Map01RadioInteractable : MonoBehaviour, IInteractable
{
    [Header("C1-S04 - Radio cu cua Nguyen An")]
    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 1.5f;

    [Header("Dialogue")]
    [SerializeField] private DialogueData radioDialogue;
    [SerializeField] private float autoNextSeconds = 10f;

    [Header("Inventory")]
    [SerializeField] private ItemBase radioItem;

    [Header("Audio - Optional")]
    [SerializeField] private AudioSource radioAudio;
    [SerializeField] private AudioClip staticNoise;

    [Header("After Completion")]
    [SerializeField] private UnityEvent onCompleted;

    private bool started;
    private bool completed;

    private void Update()
    {
        if (started || completed || player == null)
            return;

        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsTalking)
            return;

        float distance = Vector2.Distance(
            player.position,
            transform.position
        );

        if (distance > interactionDistance)
            return;

        bool pressF = Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame;

        bool rightClick = Mouse.current != null &&
            Mouse.current.rightButton.wasReleasedThisFrame;

        if (pressF || rightClick)
            Interact();
    }

    public void Interact()
    {
        if (started || completed)
            return;

        var manager = DialogueManager.Instance;

        if (manager == null || manager.IsTalking)
            return;

        if (radioDialogue == null || radioItem == null)
        {
            Debug.LogWarning(
                "[Map01 C1-S04] Chua gan Dialogue hoac Item."
            );
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning(
                "[Map01 C1-S04] Khong tim thay InventoryManager."
            );
            return;
        }

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

        InventoryManager.Instance.AddItem(radioItem, 1);

        completed = true;

        GetComponent<Collider2D>().enabled = false;

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
