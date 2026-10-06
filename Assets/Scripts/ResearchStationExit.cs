using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Exit point inside Research Station 03.
/// Returns the player to the previously stored Rung Vong scene/spawn marker.
/// </summary>
public class ResearchStationExit : MonoBehaviour
{
    [Header("Fallback Return")]
    public string fallbackScene = "Map2_RungVong_TramNghienCuu";
    public string fallbackSpawnMarker = "ResearchStationGate_Return";
    public bool loadOnTrigger = true;
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    private bool playerInside;
    private bool used;

    private void Update()
    {
        if (!loadOnTrigger && playerInside && Input.GetKeyDown(interactKey))
            ReturnToForest();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        playerInside = true;

        if (loadOnTrigger)
            ReturnToForest();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayer(other))
            playerInside = false;
    }

    public void ReturnToForest()
    {
        if (used) return;
        used = true;

        string returnScene = fallbackScene;
        string returnMarker = fallbackSpawnMarker;

        if (ResearchStationSceneConnector.HasPendingReturn)
        {
            // The static travel state is consumed by the destination scene.
            // Use the explicit fallback scene only when no entry state exists.
            returnScene = PlayerPrefs.GetString("HT_RS03_ReturnScene", fallbackScene);
            returnMarker = PlayerPrefs.GetString("HT_RS03_ReturnMarker", fallbackSpawnMarker);
        }

        ResearchStationSceneConnector.PrepareReturn(returnScene, returnMarker);

        if (!Application.CanStreamedLevelBeLoaded(returnScene))
        {
            Debug.LogError(
                $"[HẢI TÂM] Return scene '{returnScene}' is not in Build Settings.");
            used = false;
            return;
        }

        SceneManager.LoadScene(returnScene);
    }

    private static bool IsPlayer(Collider2D col)
    {
        if (col == null) return false;
        if (col.CompareTag("Player")) return true;
        if (col.attachedRigidbody != null && col.attachedRigidbody.CompareTag("Player")) return true;
        return false;
    }
}
