
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Map01VillageEntranceTrigger : MonoBehaviour
{
    [Header("C1-S06 - Ngoi lang khong nguoi")]
    [SerializeField] private DialogueData villageDialogue;
    [SerializeField] private float autoNextSeconds = 10f;

    private bool activated = false;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isPlayer = other.CompareTag("Player");

        // Ho tro Player co Collider2D tren object con
        if (!isPlayer && other.attachedRigidbody != null)
        {
            isPlayer = other.attachedRigidbody.CompareTag("Player");
        }

        if (!isPlayer || activated)
            return;

        if (villageDialogue == null)
        {
            Debug.LogWarning(
                "[Map01 C1-S06] Chua gan Village Dialogue."
            );
            return;
        }

        activated = true;
        StartCoroutine(PlayVillageIntro());
    }

    private IEnumerator PlayVillageIntro()
    {
        // Cho hoi thoai truoc ket thuc
        while (DialogueManager.Instance != null &&
               DialogueManager.Instance.IsTalking)
        {
            yield return null;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning(
                "[Map01 C1-S06] Khong tim thay DialogueManager."
            );
            activated = false;
            yield break;
        }

        // Bat dau hoi thoai khi Minh vao lang
        DialogueManager.Instance.StartDialogue(
            villageDialogue,
            true,
            autoNextSeconds
        );

        Debug.Log(
            "[Map01 C1-S06] Bat dau kham pha ngoi lang."
        );
    }
}
