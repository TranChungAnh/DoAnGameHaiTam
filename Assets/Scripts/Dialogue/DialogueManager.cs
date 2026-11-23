using UnityEngine.Events;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Trạng thái 1: Normal Dialog UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image backgroundImage;
    private float previousTimeScale = 1f;
    private bool previousAudioPause;
    //[Header("Trạng thái 2: Floating Text UI")]
    //[SerializeField] private GameObject floatingTextPanel;
    //[SerializeField] private TMP_Text floatingDialogueText;

    [Header("Cài đặt hiệu ứng chạy chữ")]
    [SerializeField] private float typewriterSpeed = 0.03f;

    [Header("Player Control")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Hiệu ứng chuyển cảnh ký ức")]
    [SerializeField] private MemoryGlitchTransition memoryGlitch;

    [Header("Tùy chọn")]
    [SerializeField] private bool hidePortraitWhenEmpty = true;
    [SerializeField] private bool hideBackgroundWhenEmpty = true;

    [Header("Kết thúc sau Boss")]
    [SerializeField] private DialogueData finalBossDialogue;
    [SerializeField] private UnityEvent onFinalBossDialogueClosed = new UnityEvent();

    private DialogueData currentDialogue;
    private int currentLineIndex;
    private bool isAutoPlay;
    private float autoPlayDelay;

    private Coroutine autoPlayCoroutine;
    private Coroutine typewriterCoroutine;
    private Coroutine transitionCoroutine;

    [SerializeField] private Rigidbody2D playerRb;

    private bool isTyping;
    private bool isEndingDialogue;

    // Lưu trạng thái thời gian trước khi hội thoại bắt đầu

    public bool IsTalking { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        /*
        if (floatingTextPanel != null)
            floatingTextPanel.SetActive(false);
        else if (floatingDialogueText != null)
            floatingDialogueText.gameObject.SetActive(false);
        */

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextLine);
            nextButton.onClick.AddListener(NextLine);
        }

        if (playerInput != null)
            playerInput.enabled = true;
    }

    private void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextLine);

        // Đảm bảo không để game bị dừng nếu DialogueManager bị hủy
        if (Instance == this && IsTalking)
        {
            ResumeGame();
        }

        if (Instance == this)
            Instance = null;
    }

    // =========================
    // PAUSE GAME
    // =========================

    private void PauseGame()
    {
        previousTimeScale = Time.timeScale;
        previousAudioPause = AudioListener.pause;

        // Dừng gameplay
        Time.timeScale = 0f;

        // Tắt toàn bộ âm thanh
        AudioListener.pause = true;
    }

    private void ResumeGame()
    {
        // Khôi phục thời gian
        Time.timeScale = previousTimeScale;

        // Khôi phục âm thanh
        AudioListener.pause = previousAudioPause;
    }

    // =========================
    // BẮT ĐẦU HỘI THOẠI
    // =========================

    public void StartDialogue(
        DialogueData dialogue,
        bool autoPlay = false,
        float delay = 3f)
    {
        if (dialogue == null ||
            dialogue.lines == null ||
            dialogue.lines.Count == 0)
        {
            Debug.LogWarning(
                "DialogueManager: DialogueData rỗng hoặc null!");
            return;
        }

        if (IsTalking)
            return;

        currentDialogue = dialogue;
        isAutoPlay = autoPlay;
        autoPlayDelay = delay;
        currentLineIndex = 0;

        IsTalking = true;
        isEndingDialogue = false;

        // Dừng toàn bộ gameplay
        PauseGame();

        LockPlayer();

        // Phát nhiễu trước khi hiện câu thoại đầu tiên
        if (memoryGlitch != null)
        {
            transitionCoroutine = StartCoroutine(
                memoryGlitch.PlayTransition(
                    () => ShowCurrentLine()
                )
            );
        }
        else
        {
            ShowCurrentLine();
        }
    }

    private void LockPlayer()
    {
        if (playerInput != null)
            playerInput.enabled = false;

        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }
    }

    private void UnlockPlayer()
    {
        if (playerInput != null)
            playerInput.enabled = true;
    }

    // =========================
    // HIỂN THỊ CÂU THOẠI
    // =========================

    private void ShowCurrentLine()
    {
        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        if (currentDialogue == null ||
            currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line =
            currentDialogue.lines[currentLineIndex];

        if (line.displayMode == DialogueDisplayMode.NormalDialog)
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);

            if (nameText != null)
                nameText.text = line.speakerName;

            StartTypewriter(dialogueText, line.dialogueText);

            UpdateCharacterPortrait(line.characterPortrait);
            UpdateDialogueBackground(line.backgroundSprite);
            UpdateNextButtonVisibility();
        }
        else
        {
            // Floating Text hiện đang tạm thời vô hiệu hóa.
        }
    }

    // =========================
    // HIỆU ỨNG CHẠY CHỮ
    // =========================

    private void StartTypewriter(
        TMP_Text targetTextComponent,
        string textToType)
    {
        if (targetTextComponent == null)
        {
            isTyping = false;
            return;
        }

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        typewriterCoroutine = StartCoroutine(
            TypewriterRoutine(targetTextComponent, textToType)
        );
    }

    private IEnumerator TypewriterRoutine(
        TMP_Text targetTextComponent,
        string textToType)
    {
        isTyping = true;
        targetTextComponent.text = "";

        foreach (char c in textToType)
        {
            targetTextComponent.text += c;

            // Sử dụng thời gian thực để chạy chữ khi game Pause
            yield return new WaitForSecondsRealtime(typewriterSpeed);
        }

        targetTextComponent.text = textToType;
        isTyping = false;
        typewriterCoroutine = null;

        if (isAutoPlay && IsTalking && !isEndingDialogue)
            autoPlayCoroutine = StartCoroutine(AutoPlayRoutine());
    }

    private IEnumerator AutoPlayRoutine()
    {
        // AutoPlay vẫn hoạt động khi game Pause
        yield return new WaitForSecondsRealtime(autoPlayDelay);

        autoPlayCoroutine = null;
        NextLine();
    }

    // =========================
    // CHUYỂN CÂU THOẠI
    // =========================

    public void NextLine()
    {
        if (!IsTalking || isEndingDialogue)
            return;

        // Nếu chữ đang chạy, hiện toàn bộ câu thoại
        if (isTyping)
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            if (currentDialogue == null)
                return;

            DialogueLine line =
                currentDialogue.lines[currentLineIndex];

            if (line.displayMode == DialogueDisplayMode.NormalDialog)
            {
                if (dialogueText != null)
                    dialogueText.text = line.dialogueText;
            }

            isTyping = false;

            if (isAutoPlay)
                autoPlayCoroutine = StartCoroutine(AutoPlayRoutine());

            return;
        }

        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        currentLineIndex++;

        if (currentDialogue == null ||
            currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    // =========================
    // KẾT THÚC HỘI THOẠI
    // =========================

    public void EndDialogue()
    {
        if (!IsTalking || isEndingDialogue)
            return;

        isEndingDialogue = true;

        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        isTyping = false;

        // Phát nhiễu lần nữa trước khi đóng hội thoại
        if (memoryGlitch != null)
        {
            transitionCoroutine = StartCoroutine(
                memoryGlitch.PlayTransition(
                    () => FinishDialogue()
                )
            );
        }
        else
        {
            FinishDialogue();
        }
    }

    private void Update()
    {
        // Bấm E để chuyển câu thoại
        if (IsTalking && !isEndingDialogue)
        {
            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                NextLine();
            }
        }
    }

    private void FinishDialogue()
    {
        bool finishedFinalBossDialogue =
            finalBossDialogue != null &&
            currentDialogue == finalBossDialogue &&
            currentLineIndex >= finalBossDialogue.lines.Count;

        CloseDialogueUI();

        currentDialogue = null;
        currentLineIndex = 0;
        isTyping = false;
        IsTalking = false;
        isEndingDialogue = false;
        transitionCoroutine = null;

        UnlockPlayer();

        // Cho gameplay chạy tiếp
        ResumeGame();

        if (finishedFinalBossDialogue)
        {
            onFinalBossDialogueClosed?.Invoke();
        }
    }

    // =========================
    // ĐÓNG GIAO DIỆN
    // =========================

    private void CloseDialogueUI()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        /*
        if (floatingTextPanel != null)
            floatingTextPanel.SetActive(false);
        else if (floatingDialogueText != null)
            floatingDialogueText.gameObject.SetActive(false);
        */
    }

    private void UpdateDialogueBackground(Sprite bgSprite)
    {
        if (backgroundImage == null)
            return;

        if (bgSprite != null)
        {
            backgroundImage.sprite = bgSprite;
            backgroundImage.gameObject.SetActive(true);
        }
        else if (hideBackgroundWhenEmpty)
        {
            backgroundImage.gameObject.SetActive(false);
        }
    }

    private void UpdateNextButtonVisibility()
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(!isAutoPlay);
    }

    private void UpdateCharacterPortrait(Sprite portrait)
    {
        if (characterImage == null)
            return;

        if (portrait != null)
        {
            characterImage.sprite = portrait;
            characterImage.gameObject.SetActive(true);
        }
        else if (hidePortraitWhenEmpty)
        {
            characterImage.gameObject.SetActive(false);
        }
    }

    public bool IsDialoguePlaying()
    {
        return IsTalking;
    }
}
