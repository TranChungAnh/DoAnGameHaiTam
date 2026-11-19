
using System.Collections;
using UnityEngine;

public class Map01OpeningSequence : MonoBehaviour
{
    [Header("C1-S01 - Tin hieu cuoi cung")]
    [SerializeField] private DialogueData openingDialogue;

    [Header("C1-S02 - Cap bo Hai Vong")]
    [SerializeField] private DialogueData beachDialogue;

    [SerializeField, Min(0f)]
    private float beachAutoPlayDelay = 2f;

    [Header("Thong bao muc tieu Map01")]
    [SerializeField] private GameObject objectivePopup;

    [SerializeField, Min(0f)]
    private float objectiveShowDuration = 3f;

    [SerializeField, Min(0f)]
    private float autoPlayDelay = 3f;

    [Header("Trang thai luu")]
    [SerializeField] private string openingDialogueID = "Map01_C1_S01";
    [SerializeField] private string beachDialogueID = "Map01_C1_S02";

    private PlayerController playerController;
    private bool previousCanMove;
    private bool movementLocked;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();

        if (playerController != null)
        {
            previousCanMove = playerController.canMove;
            playerController.canMove = false;
            movementLocked = true;
        }

        if (objectivePopup != null)
        {
            objectivePopup.SetActive(false);
        }
    }

    private IEnumerator Start()
    {
        yield return null;

        DialogueManager manager = DialogueManager.Instance;

        bool openingCompleted =
            PlayerPrefs.GetInt(openingDialogueID, 0) == 1;

        bool beachCompleted =
            PlayerPrefs.GetInt(beachDialogueID, 0) == 1;

        // Ca hai hoi thoai da hoan thanh
        if (openingCompleted && beachCompleted)
        {
            RestoreMovement();
            yield break;
        }

        if (manager == null)
        {
            Debug.LogError("[Map01] Khong tim thay DialogueManager.", this);
            RestoreMovement();
            yield break;
        }

        // Cho hoi thoai khac ket thuc
        while (manager != null && manager.IsTalking)
        {
            yield return null;
        }

        if (manager == null)
        {
            RestoreMovement();
            yield break;
        }

        // C1-S01: Tin hieu cuoi cung
        if (!openingCompleted)
        {
            if (openingDialogue == null)
            {
                Debug.LogError("[Map01 C1-S01] Thieu DialogueData.", this);
                RestoreMovement();
                yield break;
            }

            manager.StartDialogue(openingDialogue, true, autoPlayDelay);

            while (manager != null && manager.IsTalking)
            {
                yield return null;
            }

            if (manager == null)
            {
                RestoreMovement();
                yield break;
            }

            PlayerPrefs.SetInt(openingDialogueID, 1);
            PlayerPrefs.Save();

            Debug.Log("[Map01] C1-S01 completed.", this);
        }

        // C1-S02: Cap bo Hai Vong
        if (PlayerPrefs.GetInt(beachDialogueID, 0) == 0)
        {
            if (beachDialogue == null)
            {
                Debug.LogError("[Map01 C1-S02] Thieu DialogueData.", this);
                RestoreMovement();
                yield break;
            }

            manager.StartDialogue(
                beachDialogue,
                true,
                beachAutoPlayDelay
            );

            while (manager != null && manager.IsTalking)
            {
                yield return null;
            }

            if (manager == null)
            {
                RestoreMovement();
                yield break;
            }

            PlayerPrefs.SetInt(beachDialogueID, 1);
            PlayerPrefs.Save();
        }

        // Tra quyen dieu khien cho Minh
        RestoreMovement();

        // Hien thi thong bao muc tieu
        if (objectivePopup != null)
        {
            objectivePopup.SetActive(true);

            Map01ObjectivePopupVisual popupVisual =
                objectivePopup.GetComponent<Map01ObjectivePopupVisual>();

            if (popupVisual != null)
            {
                yield return StartCoroutine(
                    popupVisual.Play(objectiveShowDuration)
                );
            }
            else
            {
                yield return new WaitForSecondsRealtime(
                    objectiveShowDuration
                );
            }

            if (objectivePopup != null)
                objectivePopup.SetActive(false);
        }

        Debug.Log("[Map01] Opening sequence completed.", this);
    }

    private void RestoreMovement()
    {
        if (!movementLocked) return;

        if (playerController != null)
        {
            playerController.canMove = previousCanMove;
        }

        movementLocked = false;
    }

    private void OnDisable()
    {
        RestoreMovement();
    }
}
