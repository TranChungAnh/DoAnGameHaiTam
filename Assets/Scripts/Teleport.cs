
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

[System.Serializable]
public class RequiredArtifact
{
    [Tooltip("Kéo file ScriptableObject của cổ vật vào đây")]
    public ItemBase artifact;

    [Tooltip("Số lượng cổ vật cần có trong kho")]
    public int requiredAmount = 1;
}

public class Teleport : MonoBehaviour
{
    [Tooltip("Kéo scene đích từ cửa sổ Project vào đây.")]
    [SerializeField] private Object targetScene;

    [Tooltip("Tên scene trong Build Settings. Để trống để dùng scene đã kéo vào Target Scene.")]
    [SerializeField] private string targetSceneName;

    [Tooltip("Tên GameObject đánh dấu vị trí Minh xuất hiện trong scene đích.")]
    [SerializeField] private string destinationSpawnPoint;

    [Tooltip("Danh sách các cổ vật cần thiết để mở cổng dịch chuyển")]
    [SerializeField] private List<RequiredArtifact> requiredArtifacts = new List<RequiredArtifact>();

    [Header("Thông báo thiếu cổ vật")]
    [SerializeField] private DialogueData missingArtifactDialogue;

    // ==========================================
    // MAP01 - C1-S09
    // ==========================================

    [Header("Map01 - C1-S09")]

    [Tooltip("Chỉ bật tại cổng chuyển từ Map01 sang Map02")]
    [SerializeField] private bool playMap01S09 = false;

    [Tooltip("Dữ liệu hội thoại Map01_C1_S09")]
    [SerializeField] private DialogueData map01S09Dialogue;

    [Tooltip("Thời gian tự động chuyển câu thoại")]
    [SerializeField] private float autoNextSeconds = 10f;

    private const string Map01S09Key = "Map01_C1_S09_Completed";

    private bool isLoading;

    // ==========================================
    // KIỂM TRA KHI PLAYER VÀO TRIGGER
    // ==========================================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLoading || !collision.CompareTag("Player"))
            return;

        if (!HasAllRequiredArtifacts())
        {
            ShowMissingArtifactDialogue();
            return;
        }

        string sceneName = ResolveSceneName();

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning(
                $"Cổng '{name}' chưa được gán scene đích.",
                this
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"Cổng '{name}' không tải được scene '{sceneName}'. Kiểm tra Scenes In Build.",
                this
            );

            return;
        }

        isLoading = true;

        // Kiểm tra có cần phát hội thoại C1-S09 không
        if (playMap01S09 &&
            PlayerPrefs.GetInt(Map01S09Key, 0) == 0)
        {
            StartCoroutine(PlayMap01S09ThenLoad(sceneName));
        }
        else
        {
            // Đã xem hội thoại hoặc là cổng khác
            LoadTargetScene(sceneName);
        }
    }

    // ==========================================
    // PHÁT HỘI THOẠI C1-S09
    // ==========================================

    private IEnumerator PlayMap01S09ThenLoad(string sceneName)
    {
        DialogueManager manager = DialogueManager.Instance;

        if (manager == null)
        {
            Debug.LogError(
                "[Map01 C1-S09] Không tìm thấy DialogueManager!",
                this
            );

            isLoading = false;
            yield break;
        }

        if (map01S09Dialogue == null ||
            map01S09Dialogue.lines == null ||
            map01S09Dialogue.lines.Count == 0)
        {
            Debug.LogError(
                "[Map01 C1-S09] Chưa gán dữ liệu hội thoại hoặc hội thoại rỗng!",
                this
            );

            isLoading = false;
            yield break;
        }

        // Chờ hội thoại khác kết thúc nếu đang phát
        while (manager != null && manager.IsTalking)
        {
            yield return null;
        }

        if (manager == null)
        {
            isLoading = false;
            yield break;
        }

        Debug.Log("[Map01 C1-S09] Bắt đầu hội thoại.");

        // Phát hội thoại C1-S09
        manager.StartDialogue(
            map01S09Dialogue,
            true,
            autoNextSeconds
        );

        // Kiểm tra hội thoại đã được kích hoạt
        if (!manager.IsTalking)
        {
            Debug.LogWarning(
                "[Map01 C1-S09] Không thể bắt đầu hội thoại.",
                this
            );

            isLoading = false;
            yield break;
        }

        yield return null;

        // Chờ toàn bộ hội thoại kết thúc
        while (manager != null && manager.IsTalking)
        {
            yield return null;
        }

        if (manager == null)
        {
            isLoading = false;
            yield break;
        }

        // Đánh dấu đã hoàn thành hội thoại
        PlayerPrefs.SetInt(Map01S09Key, 1);
        PlayerPrefs.Save();

        Debug.Log(
            "[Map01 C1-S09] Hoàn thành hội thoại. Chuyển sang Map02."
        );

        // Chuyển map sau khi hội thoại kết thúc
        LoadTargetScene(sceneName);
    }

    // ==========================================
    // CHUYỂN SCENE
    // ==========================================

    private void LoadTargetScene(string sceneName)
    {
        // Giữ nguyên cơ chế xác định điểm xuất hiện
        ResearchStationSceneConnector.PreparePortalArrival(
            sceneName,
            destinationSpawnPoint
        );

        SceneManager.LoadScene(sceneName);
    }

    // ==========================================
    // THÔNG BÁO THIẾU CỔ VẬT
    // ==========================================

    private void ShowMissingArtifactDialogue()
    {
        if (missingArtifactDialogue == null)
        {
            Debug.LogWarning(
                "Chưa gán DialogueData thông báo thiếu cổ vật!",
                this
            );

            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning(
                "Không tìm thấy DialogueManager!",
                this
            );

            return;
        }

        DialogueManager.Instance.StartDialogue(
            missingArtifactDialogue,
            false,
            3f
        );
    }

    // ==========================================
    // XÁC ĐỊNH TÊN SCENE
    // ==========================================

    private string ResolveSceneName()
    {
        if (!string.IsNullOrWhiteSpace(targetSceneName))
            return targetSceneName.Trim();

        return targetScene != null
            ? targetScene.name
            : string.Empty;
    }

    // ==========================================
    // KIỂM TRA CỔ VẬT
    // ==========================================

    private bool HasAllRequiredArtifacts()
    {
        InventoryManager inventory = InventoryManager.Instance;

        if (inventory == null)
        {
            Debug.LogError(
                $"Cổng '{name}' không tìm thấy InventoryManager.",
                this
            );

            return false;
        }

        foreach (RequiredArtifact requirement in requiredArtifacts)
        {
            if (requirement == null || requirement.artifact == null)
                continue;

            int count = inventory.GetItemCount(
                requirement.artifact
            );

            if (count < requirement.requiredAmount)
            {
                Debug.LogWarning(
                    $"Thiếu {requirement.artifact.itemName}: " +
                    $"cần {requirement.requiredAmount}, hiện có {count}.",
                    this
                );

                return false;
            }
        }

        return true;
    }

#if UNITY_EDITOR
    internal void StoreSceneNameForBuild()
    {
        targetSceneName = ResolveSceneName();
    }
#endif
}

#if UNITY_EDITOR

// Ghi tên scene đích vào từng cổng khi Unity tạo bản build.
public sealed class TeleportSceneBuildProcessor : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        if (report == null)
            return;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Teleport portal in root.GetComponentsInChildren<Teleport>(true))
            {
                portal.StoreSceneNameForBuild();
            }
        }
    }
}

#endif
