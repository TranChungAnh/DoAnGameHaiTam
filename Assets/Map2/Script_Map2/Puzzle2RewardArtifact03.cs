using UnityEngine;
using UnityEngine.Events;

public class Puzzle2RewardArtifact03 : MonoBehaviour
{
    [SerializeField] private Map2Puzzle2Manager puzzleManager;

    [SerializeField] private GameObject artifactObject;

    public UnityEvent OnArtifact03Available;
    public UnityEvent OnArtifact03Collected;

    private bool available;
    private bool collected;

    private void Awake()
    {
        if (artifactObject == null)
        {
            artifactObject = gameObject;
        }

        artifactObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (puzzleManager != null)
        {
            puzzleManager.OnPuzzleSolved
                .AddListener(MakeAvailable);
        }
    }

    private void OnDisable()
    {
        if (puzzleManager != null)
        {
            puzzleManager.OnPuzzleSolved
                .RemoveListener(MakeAvailable);
        }
    }

    private void MakeAvailable()
    {
        available = true;

        artifactObject.SetActive(true);

        OnArtifact03Available?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!available)
            return;

        if (!other.CompareTag("Player"))
            return;

        Collect();
    }

    private void Collect()
    {
        if (collected)
            return;

        collected = true;

        OnArtifact03Collected?.Invoke();

        artifactObject.SetActive(false);
    }
}