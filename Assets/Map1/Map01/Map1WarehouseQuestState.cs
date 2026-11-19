using UnityEngine;

/// <summary>
/// Runtime state for the Map 1 warehouse. Attach ONCE to the persistent
/// Map01 travel object. This is not a disk save system.
/// </summary>
public class Map1WarehouseQuestState : MonoBehaviour
{
    public static Map1WarehouseQuestState Instance { get; private set; }

    public bool DocumentCollected { get; private set; }
    public bool ChestKeyCollected { get; private set; }
    public bool ChestOpened { get; private set; }
    public bool Artifact01Collected { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[Map1] More than one WarehouseQuestState found.", this);
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void MarkDocumentCollected() => DocumentCollected = true;
    public void MarkChestKeyCollected() => ChestKeyCollected = true;
    public void MarkChestOpened() => ChestOpened = true;
    public void MarkArtifact01Collected() => Artifact01Collected = true;
}
