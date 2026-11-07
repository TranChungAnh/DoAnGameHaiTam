
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Map01VillageMemoryTrigger : MonoBehaviour
{
    [Header("C1-S05 - Loi dan trong ky uc")]
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private float autoNextSeconds = 10f;

    [Header("Black Screen Effect")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private float fadeDuration = 0.15f;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip memoryVoice;

    [Header("On Completed")]
    [SerializeField] private UnityEvent onCompleted;

    private bool activated;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;

        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated || !other.CompareTag("Player"))
            return;

        if (dialogueData == null ||
            DialogueManager.Instance == null)
        {
            Debug.LogWarning(
                "[Map01 C1-S05] Missing dialogue setup."
            );
            return;
        }

        if (DialogueManager.Instance.IsTalking)
            return;

        activated = true;
        StartCoroutine(PlayMemorySequence());
    }

    private IEnumerator PlayMemorySequence()
    {
        // Man hinh toi di trong chop mat
        if (blackScreen != null)
        {
            yield return FadeScreen(0f, 1f);

            yield return new WaitForSecondsRealtime(0.1f);

            yield return FadeScreen(1f, 0f);
        }

        // Phat Audio 2 neu da co
        if (audioSource != null && memoryVoice != null)
        {
            audioSource.PlayOneShot(memoryVoice);
        }

        // Bat dau hoi thoai
        DialogueManager.Instance.StartDialogue(
            dialogueData,
            true,
            autoNextSeconds
        );

        yield return null;

        // Cho hoi thoai ket thuc
        while (DialogueManager.Instance != null &&
               DialogueManager.Instance.IsTalking)
        {
            yield return null;
        }

        // Cap nhat muc tieu
        onCompleted?.Invoke();

        Debug.Log("[Map01 C1-S05] Completed.");
    }

    private IEnumerator FadeScreen(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            blackScreen.alpha = Mathf.Lerp(
                from,
                to,
                Mathf.Clamp01(elapsed / fadeDuration)
            );

            yield return null;
        }

        blackScreen.alpha = to;
    }
}
