using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Runtime scene connector for the independent Research Station 03 map.
/// Can be used on the gate inside Rung Vong or on another entry point.
/// Stores the scene/spawn marker to return to when the station is exited.
/// </summary>
public class ResearchStationSceneConnector : MonoBehaviour
{
    [Header("Scene Travel")]
    public string targetScene = "Map2B_ResearchStation03";
    public string returnSceneName = "Map2_RungVong_TramNghienCuu";
    public string returnSpawnMarkerName = "ResearchStationGate_Return";
    public bool loadOnTrigger = true;
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    private bool playerInside;
    private bool used;

    private static string pendingReturnScene;
    private static string pendingReturnMarker;

    private const string PrefReturnScene = "HT_RS03_ReturnScene";
    private const string PrefReturnMarker = "HT_RS03_ReturnMarker";

    public static bool HasPendingReturn =>
        !string.IsNullOrEmpty(pendingReturnScene) ||
        PlayerPrefs.HasKey(PrefReturnScene);

    private void Update()
    {
        if (!loadOnTrigger && playerInside && Input.GetKeyDown(interactKey))
            TravelToStation();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        playerInside = true;

        if (loadOnTrigger)
            TravelToStation();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayer(other))
            playerInside = false;
    }

    public void TravelToStation()
    {
        if (used) return;
        used = true;

        string currentScene = SceneManager.GetActiveScene().name;
        string resolvedReturnScene =
            string.IsNullOrEmpty(returnSceneName)
                ? currentScene
                : returnSceneName;

        pendingReturnScene = resolvedReturnScene;
        pendingReturnMarker = returnSpawnMarkerName;

        PlayerPrefs.SetString(PrefReturnScene, resolvedReturnScene);
        PlayerPrefs.SetString(PrefReturnMarker, returnSpawnMarkerName);
        PlayerPrefs.Save();

        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            Debug.LogError(
                $"[HẢI TÂM] Research Station scene '{targetScene}' is not in Build Settings.");
            used = false;
            return;
        }

        SceneManager.LoadScene(targetScene);
    }

    public static void PrepareReturn(string sceneName, string markerName)
    {
        pendingReturnScene = sceneName;
        pendingReturnMarker = markerName;
        PlayerPrefs.SetString(PrefReturnScene, sceneName);
        PlayerPrefs.SetString(PrefReturnMarker, markerName);
        PlayerPrefs.Save();
    }

    public static bool TryConsumeReturn(string sceneName, out string markerName)
    {
        markerName = null;

        string targetScene = pendingReturnScene;
        string targetMarker = pendingReturnMarker;

        if (string.IsNullOrEmpty(targetScene))
            targetScene = PlayerPrefs.GetString(PrefReturnScene, string.Empty);
        if (string.IsNullOrEmpty(targetMarker))
            targetMarker = PlayerPrefs.GetString(PrefReturnMarker, string.Empty);

        if (string.IsNullOrEmpty(targetScene) || targetScene != sceneName)
            return false;

        markerName = targetMarker;
        pendingReturnScene = null;
        pendingReturnMarker = null;

        PlayerPrefs.DeleteKey(PrefReturnScene);
        PlayerPrefs.DeleteKey(PrefReturnMarker);
        PlayerPrefs.Save();
        return true;
    }

    private static bool IsPlayer(Collider2D col)
    {
        if (col == null) return false;
        if (col.CompareTag("Player")) return true;
        if (col.attachedRigidbody != null && col.attachedRigidbody.CompareTag("Player")) return true;
        return false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InstallSceneReturnResolver()
    {
        SceneManager.sceneLoaded -= ResolvePendingReturn;
        SceneManager.sceneLoaded += ResolvePendingReturn;
    }

    private static void ResolvePendingReturn(Scene scene, LoadSceneMode mode)
    {
        if (!TryConsumeReturn(scene.name, out string markerName))
            return;

        if (string.IsNullOrEmpty(markerName))
            return;

        GameObject marker = GameObject.Find(markerName);
        if (marker == null)
        {
            Debug.LogWarning($"[HẢI TÂM] Return marker '{markerName}' not found in scene '{scene.name}'.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[HẢI TÂM] Player tag/object not found when returning from Research Station.");
            return;
        }

        player.transform.position = marker.transform.position;
    }
}
