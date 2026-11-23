using UnityEngine;
using UnityEngine.SceneManagement;

public class ResearchStationSceneConnector : MonoBehaviour
{
    [Header("Scene Travel")]
    public string targetScene = "Map2B_ResearchStation03";
    public string returnSceneName = "Map2_RungVong_TramNghienCuu (1)";
    public string returnSpawnMarkerName = "Spawn_Map2_From_Station";
    public bool loadOnTrigger = true;
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    private const string PrefReturnScene = "HT_RS03_ReturnScene";
    private const string PrefReturnMarker = "HT_RS03_ReturnMarker";

    private bool playerInside;
    private bool used;

    // Điểm đến của cổng giữa hai map, chỉ dùng cho một lần chuyển scene.
    private static string portalScene;
    private static string portalMarker;

    // ResearchStationExit.cs dùng thuộc tính này khi rời trạm.
    public static bool HasPendingReturn => PlayerPrefs.HasKey(PrefReturnScene);

    private void Update()
    {
        if (!loadOnTrigger && playerInside && Input.GetKeyDown(interactKey))
            TravelToStation();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = true;

        if (loadOnTrigger)
            TravelToStation();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayer(other))
            playerInside = false;
    }

    private bool IsPlayer(Collider2D other)
    {
        if (other == null)
            return false;

        if (other.CompareTag(playerTag))
            return true;

        Rigidbody2D body = other.attachedRigidbody;
        return body != null && body.CompareTag(playerTag);
    }

    public void TravelToStation()
    {
        if (used)
            return;

        string destination = targetScene?.Trim();

        if (string.IsNullOrEmpty(destination) ||
            !Application.CanStreamedLevelBeLoaded(destination))
        {
            Debug.LogError(
                $"[HẢI TÂM] Không tải được scene '{destination}'. Kiểm tra Scenes In Build.",
                this
            );
            return;
        }

        string returnTo = returnSceneName?.Trim();

        // Các component cũ có thể còn lưu tên Rừng Vọng thiếu phần " (1)".
        if (string.IsNullOrEmpty(returnTo) ||
            !Application.CanStreamedLevelBeLoaded(returnTo))
        {
            returnTo = SceneManager.GetActiveScene().name;
        }

        used = true;
        PrepareReturn(returnTo, returnSpawnMarkerName);
        SceneManager.LoadScene(destination);
    }

    // Teleport.cs gọi hàm này ngay trước khi tải map đích.
    public static void PreparePortalArrival(string sceneName, string markerName)
    {
        portalScene = sceneName?.Trim();
        portalMarker = markerName?.Trim();
    }

    // Hai khóa này cũng được ResearchStationExit.cs đọc khi rời trạm.
    public static void PrepareReturn(string sceneName, string markerName)
    {
        PlayerPrefs.SetString(PrefReturnScene, sceneName?.Trim() ?? string.Empty);
        PlayerPrefs.SetString(PrefReturnMarker, markerName?.Trim() ?? string.Empty);
        PlayerPrefs.Save();
    }

    public static bool TryConsumeReturn(string sceneName, out string markerName)
    {
        markerName = null;
        string savedScene = PlayerPrefs.GetString(PrefReturnScene, string.Empty);

        if (string.IsNullOrEmpty(savedScene) || savedScene != sceneName)
            return false;

        markerName = PlayerPrefs.GetString(PrefReturnMarker, string.Empty);

        PlayerPrefs.DeleteKey(PrefReturnScene);
        PlayerPrefs.DeleteKey(PrefReturnMarker);
        PlayerPrefs.Save();
        return true;
    }

    private static void ClearPortalArrival()
    {
        portalScene = null;
        portalMarker = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InstallSceneReturnResolver()
    {
        ClearPortalArrival();
        SceneManager.sceneLoaded -= ResolvePendingReturn;
        SceneManager.sceneLoaded += ResolvePendingReturn;
    }

    private static void ResolvePendingReturn(Scene scene, LoadSceneMode mode)
    {
        TryConsumeReturn(scene.name, out string markerName);

        if (mode == LoadSceneMode.Single)
        {
            bool isPortalDestination =
                portalScene == scene.name ||
                portalScene == scene.path ||
                scene.path == portalScene + ".unity";

            if (isPortalDestination && !string.IsNullOrEmpty(portalMarker))
                markerName = portalMarker;

            ClearPortalArrival();
        }

        // GameMamager sẽ khôi phục tọa độ nếu người chơi đang tải bản lưu.
        if (TemporarySaveData.pendingSaveData != null ||
            string.IsNullOrEmpty(markerName))
        {
            return;
        }

        GameObject marker = GameObject.Find(markerName);
        if (marker == null || marker.scene != scene)
        {
            Debug.LogWarning(
                $"[HẢI TÂM] Không tìm thấy điểm '{markerName}' trong scene '{scene.name}'. " +
                "Kiểm tra tên và trạng thái Active của GameObject."
            );
            return;
        }

        Player player = null;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (!root.activeInHierarchy)
                continue;

            player = root.GetComponentInChildren<Player>();
            if (player != null)
                break;
        }
        if (player == null)
        {
            Debug.LogWarning($"[HẢI TÂM] Không tìm thấy Player trong scene '{scene.name}'.");
            return;
        }

        Vector3 position = marker.transform.position;
        position.z = player.transform.position.z;

        if (player.TryGetComponent<Rigidbody2D>(out var body))
        {
            body.velocity = Vector2.zero;
            body.position = new Vector2(position.x, position.y);
        }

        player.transform.position = position;
        Debug.Log($"[Cổng] {scene.name} | {player.name} → {markerName} | {player.transform.position}", player);
    }
}