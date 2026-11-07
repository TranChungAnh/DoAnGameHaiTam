using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Tooltip("Kéo thả Scene vào đây")]
    [SerializeField] private Object targetScene;

    [Tooltip("Danh sách các cổ vật cần thiết để mở cổng dịch chuyển")]
    [SerializeField] private List<RequiredArtifact> requiredArtifacts = new List<RequiredArtifact>();

    private GameMamager GM;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        GM = GameObject.Find("GameMamager")?.GetComponent<GameMamager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Kiểm tra xem đã đủ cổ vật chưa
            if (HasAllRequiredArtifacts())
            {
                if (targetScene != null)
                {
                    string sceneName = targetScene.name;
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    Debug.LogWarning("⚠ Chưa kéo thả Scene vào cổng dịch chuyển!", gameObject);
                }
            }
            else
            {
                Debug.LogWarning("⛔ Bạn chưa thu thập đủ cổ vật để chuyển sang map mới!");
            }
        }
    }

 
    private bool HasAllRequiredArtifacts()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("❌ Không tìm thấy InventoryManager.Instance!");
            return false;
        }

        foreach (var req in requiredArtifacts)
        {
            if (req.artifact == null) continue;

            int currentAmount = InventoryManager.Instance.GetItemCount(req.artifact);
            if (currentAmount < req.requiredAmount)
            {
                Debug.Log($"❌ Thiếu {req.artifact.itemName}: Cần {req.requiredAmount}, hiện có {currentAmount}");
                return false;
            }
        }

        return true;
    }
}