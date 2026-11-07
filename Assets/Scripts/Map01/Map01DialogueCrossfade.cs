using UnityEngine;
using UnityEngine.UI;

public class Map01DialogueCrossfade : MonoBehaviour
{
    [Header("Anh hoi thoai Map01")]
    [SerializeField] private Image currentImage;
    [SerializeField] private Image previousImage;

    [Header("Nen den")]
    [SerializeField] private Image blackBackdrop;

    [Header("Thoi gian chuyen canh")]
    [SerializeField, Min(0.05f)] private float fadeOutDuration = 0.4f;
    [SerializeField, Min(0.05f)] private float fadeInDuration = 0.4f;

    private enum FadeState
    {
        None,
        FadeOutPrevious,
        FadeInCurrent
    }

    private FadeState fadeState = FadeState.None;

    private Sprite lastSprite;
    private Color currentBaseColor;
    private Color previousBaseColor;

    private float timer;

    private void Awake()
    {
        if (currentImage == null || previousImage == null)
        {
            Debug.LogError("[Map01] FadeThroughBlack: thieu Image reference.", this);
            enabled = false;
            return;
        }

        currentBaseColor = currentImage.color;
        previousBaseColor = previousImage.color;

        previousImage.raycastTarget = false;
        currentImage.raycastTarget = false;

        previousImage.enabled = false;

        if (blackBackdrop != null)
        {
            blackBackdrop.raycastTarget = false;
            blackBackdrop.color = Color.black;
            blackBackdrop.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        bool showingMemory =
            currentImage.gameObject.activeInHierarchy &&
            currentImage.sprite != null;

        if (!showingMemory)
        {
            ResetAll();
            return;
        }

        Sprite newSprite = currentImage.sprite;

        // Phat hien DialogueManager vua doi sang anh moi
        if (newSprite != lastSprite)
        {
            BeginTransition(newSprite);
        }

        UpdateTransition();
    }

    private void BeginTransition(Sprite newSprite)
    {
        // Luu anh cu vao previous de fade-out
        Sprite oldSprite = lastSprite;
        lastSprite = newSprite;

        timer = 0f;

        SetBackdropActive(true);

        // Mac dinh anh moi an di luc moi doi
        SetAlpha(currentImage, currentBaseColor, 0f);

        if (oldSprite != null)
        {
            previousImage.sprite = oldSprite;
            previousImage.enabled = true;
            SetAlpha(previousImage, previousBaseColor, 1f);

            fadeState = FadeState.FadeOutPrevious;
        }
        else
        {
            // Neu la anh dau tien, khong co anh cu thi vao thang fade-in
            previousImage.sprite = null;
            previousImage.enabled = false;

            fadeState = FadeState.FadeInCurrent;
        }
    }

    private void UpdateTransition()
    {
        switch (fadeState)
        {
            case FadeState.None:
                break;

            case FadeState.FadeOutPrevious:
                timer += Time.unscaledDeltaTime;
                {
                    float t = Mathf.Clamp01(timer / fadeOutDuration);
                    t = Mathf.SmoothStep(0f, 1f, t);

                    // Anh cu mo dan
                    SetAlpha(previousImage, previousBaseColor, 1f - t);

                    if (t >= 1f)
                    {
                        previousImage.enabled = false;
                        previousImage.sprite = null;

                        timer = 0f;
                        fadeState = FadeState.FadeInCurrent;
                    }
                }
                break;

            case FadeState.FadeInCurrent:
                timer += Time.unscaledDeltaTime;
                {
                    float t = Mathf.Clamp01(timer / fadeInDuration);
                    t = Mathf.SmoothStep(0f, 1f, t);

                    // Anh moi ro dan
                    SetAlpha(currentImage, currentBaseColor, t);

                    if (t >= 1f)
                    {
                        currentImage.color = currentBaseColor;
                        fadeState = FadeState.None;
                    }
                }
                break;
        }

        // Tat nen den neu khong con transition
        if (fadeState == FadeState.None)
        {
            SetBackdropActive(true);
            // Giu nen den dang bat trong luc hien anh ky uc
            // Neu ban muon tat nen den sau khi fade xong thi doi thanh false
        }
    }

    private void SetAlpha(Image image, Color baseColor, float alphaFactor)
    {
        if (image == null) return;

        Color c = baseColor;
        c.a = baseColor.a * alphaFactor;
        image.color = c;
    }

    private void SetBackdropActive(bool active)
    {
        if (blackBackdrop != null && blackBackdrop.gameObject.activeSelf != active)
        {
            blackBackdrop.gameObject.SetActive(active);
        }
    }

    private void ResetAll()
    {
        fadeState = FadeState.None;
        timer = 0f;
        lastSprite = null;

        if (previousImage != null)
        {
            previousImage.enabled = false;
            previousImage.sprite = null;
            previousImage.color = previousBaseColor;
        }

        if (currentImage != null)
        {
            currentImage.color = currentBaseColor;
        }

        SetBackdropActive(false);
    }

    private void OnDisable()
    {
        ResetAll();
    }
}