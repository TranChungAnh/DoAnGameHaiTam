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

    [Tooltip("Danh sách các cổ vật cần thiết để mở cổng dịch chuyển")]
    [SerializeField] private List<RequiredArtifact> requiredArtifacts = new List<RequiredArtifact>();

    private bool isLoading;
    //[SerializeField] private string destinationSpawnPoint;
    //private static string nextSpawnPoint;
    [Header("Thông báo thiếu cổ vật")]
    [SerializeField] private DialogueData missingArtifactDialogue;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLoading || !collision.CompareTag("Player"))
            return;

        // Kiểm tra cổ vật
        if (!HasAllRequiredArtifacts())
        {
            ShowMissingArtifactDialogue();
            return;
        }

        string sceneName = ResolveSceneName();

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"Cổng '{name}' chưa được gán scene đích.", this);
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

        //nextSpawnPoint = destinationSpawnPoint;

        //SceneManager.sceneLoaded -= OnSceneLoaded;
        //SceneManager.sceneLoaded += OnSceneLoaded;

        isLoading = true;
        SceneManager.LoadScene(sceneName);
    }
    private void ShowMissingArtifactDialogue()
    {
        if (missingArtifactDialogue == null)
        {
            Debug.LogWarning("Chưa gán DialogueData thông báo thiếu cổ vật!", this);
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy DialogueManager!");
            return;
        }

        DialogueManager.Instance.StartDialogue(
            missingArtifactDialogue,
            false,
            3f
        );
    }
    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    //SceneManager.sceneLoaded -= OnSceneLoaded;

    //    GameObject player = GameObject.FindGameObjectWithTag("Player");
    //    GameObject spawnPoint = GameObject.Find(nextSpawnPoint);

    //    if (player == null)
    //    {
    //        Debug.LogError("Không tìm thấy Player trong Scene mới!");
    //        return;
    //    }

    //    if (spawnPoint == null)
    //    {
    //        Debug.LogError("Không tìm thấy Spawn Point: " + nextSpawnPoint);
    //        return;
    //    }

    //    player.transform.position = spawnPoint.transform.position;
    //}
    private string ResolveSceneName()
    {
        if (!string.IsNullOrWhiteSpace(targetSceneName))
            return targetSceneName.Trim();

        return targetScene != null ? targetScene.name : string.Empty;
    }

    private bool HasAllRequiredArtifacts()
    {
        InventoryManager inventory = InventoryManager.Instance;

        if (inventory == null)
        {
            Debug.LogError($"Cổng '{name}' không tìm thấy InventoryManager.", this);
            return false;
        }

        foreach (RequiredArtifact requirement in requiredArtifacts)
        {
            if (requirement == null || requirement.artifact == null)
                continue;

            int count = inventory.GetItemCount(requirement.artifact);

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
// Ghi tên scene đích của từng cổng vào bản build khi Unity xử lý map.
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
                portal.StoreSceneNameForBuild();
        }
    }
}
#endif