using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Panel")]
    [SerializeField] private GameObject dialoguePanel;

    [Header("UI")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Button nextButton;

    [Header("Player Control")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Tùy chọn")]
    [SerializeField] private bool hidePortraitWhenEmpty = true;

    private DialogueData currentDialogue;
    private int currentLineIndex;

    public bool IsTalking { get; private set; }

    public DialogueData CurrentDialogue => currentDialogue;
    public int CurrentLineIndex => currentLineIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextLine);
            nextButton.onClick.AddListener(NextLine);
        }

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }
    }

    private void OnDestroy()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextLine);
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogue == null)
        {
            Debug.LogWarning("DialogueManager: DialogueData đang bị null!");
            return;
        }

        if (dialogue.lines == null || dialogue.lines.Count == 0)
        {
            Debug.LogWarning(
                $"DialogueManager: Dialogue '{dialogue.name}' không có nội dung!"
            );
            return;
        }

        if (IsTalking)
        {
            return;
        }

        currentDialogue = dialogue;
        currentLineIndex = 0;
        IsTalking = true;

        LockPlayer();

        OpenDialogueUI();
        ShowCurrentLine();
    }

    private void LockPlayer()
    {
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
    }

    private void UnlockPlayer()
    {
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }
    }

    private void ShowCurrentLine()
    {
        if (currentDialogue == null)
        {
            EndDialogue();
            return;
        }

        if (currentDialogue.lines == null ||
            currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];

        if (nameText != null)
        {
            nameText.text = line.speakerName;
        }

        if (dialogueText != null)
        {
            dialogueText.text = line.dialogueText;
        }

        UpdateCharacterPortrait(line.characterPortrait);
    }

    public void NextLine()
    {
        if (!IsTalking)
        {
            return;
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

    public void EndDialogue()
    {
        IsTalking = false;

        currentDialogue = null;
        currentLineIndex = 0;

        UnlockPlayer();
        CloseDialogueUI();
    }

    private void OpenDialogueUI()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
    }

    private void CloseDialogueUI()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private void UpdateCharacterPortrait(Sprite portrait)
    {
        if (characterImage == null)
        {
            return;
        }

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