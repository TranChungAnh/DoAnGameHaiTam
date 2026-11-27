
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Map01ObjectivePopupVisual : MonoBehaviour
{
    [Header("Hieu ung Objective Map01")]
    [SerializeField] private float fadeInDuration = 0.35f;
    [SerializeField] private float fadeOutDuration = 0.45f;
    [SerializeField] private float slideDistance = 20f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    [Header("Hiện thông báo khi vào map")]
    [SerializeField] private bool showOnMapEnter;

    private IEnumerator Start()
    {
        if (!showOnMapEnter)
            yield break;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        yield return null;

        while (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking)
            yield return null;

        yield return Play(4f);
        gameObject.SetActive(false);
    }

    public IEnumerator Play(float totalDuration)
    {
        float fadeIn = Mathf.Max(0.05f, fadeInDuration);
        float fadeOut = Mathf.Max(0.05f, fadeOutDuration);

        totalDuration = Mathf.Max(
            totalDuration,
            fadeIn + fadeOut
        );

        float elapsed = 0f;

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        Vector2 startPosition =
            originalPosition + Vector2.up * slideDistance;

        rectTransform.anchoredPosition = startPosition;

        while (elapsed < totalDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float inT = Mathf.Clamp01(elapsed / fadeIn);
            float outT = Mathf.Clamp01(
                (totalDuration - elapsed) / fadeOut
            );

            canvasGroup.alpha = Mathf.Min(inT, outT);

            float smooth = Mathf.SmoothStep(0f, 1f, inT);

            rectTransform.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    originalPosition,
                    smooth
                );

            yield return null;
        }

        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = originalPosition;
    }
}
