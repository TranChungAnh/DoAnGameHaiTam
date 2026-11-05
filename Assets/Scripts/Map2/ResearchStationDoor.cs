using UnityEngine;
using UnityEngine.Events;

public class ResearchStationDoor : MonoBehaviour
{
    [SerializeField] private Map2Puzzle2Manager puzzleManager;

    [Header("Optional")]
    [SerializeField] private Collider2D blockingCollider;
    [SerializeField] private GameObject visualDoor;

    public UnityEvent OnDoorOpened;

    private bool opened;

    private void OnEnable()
    {
        if (puzzleManager != null)
        {
            puzzleManager.OnPuzzleSolved
                .AddListener(OpenDoor);
        }
    }

    private void OnDisable()
    {
        if (puzzleManager != null)
        {
            puzzleManager.OnPuzzleSolved
                .RemoveListener(OpenDoor);
        }
    }

    public void OpenDoor()
    {
        if (opened)
            return;

        opened = true;

        if (blockingCollider != null)
        {
            blockingCollider.enabled = false;
        }

        if (visualDoor != null)
        {
            visualDoor.SetActive(false);
        }

        OnDoorOpened?.Invoke();
    }
}