
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

    private PlayerController playerController;
    private bool previousCanMove;
    private bool movementLocked;

    private void Awake()
    {
        // Khoa di chuyen truoc khi bat dau game
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
        // Cho cac Manager khoi tao
        yield return null;

        DialogueManager manager = DialogueManager.Instance;

        if (manager == null || openingDialogue == null)
        {
            Debug.LogError(
                "[Map01 C1-S01] Thieu DialogueManager hoac DialogueData.",
                this
            );

            RestoreMovement();
            yield break;
        }

        // Khong phat chong len mot hoi thoai khac
        while (manager != null && manager.IsTalking)
        {
            yield return null;
        }

        if (manager == null)
        {
            RestoreMovement();
            yield break;
        }

        // Bat dau canh mo dau, tu dong chuyen cau
        manager.StartDialogue(
            openingDialogue,
            true,
            autoPlayDelay
        );

        yield return null;

        if (!manager.IsTalking)
        {
            Debug.LogWarning(
                "[Map01 C1-S01] Khong khoi dong duoc hoi thoai.",
                this
            );
        }

        // Cho den khi canh hoi thoai ket thuc

        while (manager != null && manager.IsTalking)
        {
            yield return null;
        }

        // C1-S01 da ket thuc
        Debug.Log("[Map01] C1-S01 completed.", this);

        // Bat dau C1-S02
        if (manager != null && beachDialogue != null)
        {
            manager.StartDialogue(
                beachDialogue,
                true,
                beachAutoPlayDelay
            );

            yield return null;

            // Cho C1-S02 ket thuc
            while (manager != null && manager.IsTalking)
            {
                yield return null;
            }
        }

        // Tra quyen dieu khien cho Minh
        RestoreMovement();



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



        Debug.Log("[Map01] C1-S02 completed.", this);

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
