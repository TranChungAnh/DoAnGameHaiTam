using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    [Header("Vị trí spawn (đặt empty GameObject làm con của Player)")]
    public Transform spawnPoint;

    private GameObject currentSpawned; // Giữ reference item đã spawn

    public void SpawnItem(ItemBase item)
    {
        // Xóa item cũ nếu có
        if (currentSpawned != null)
        {
            Destroy(currentSpawned);
            currentSpawned = null;
        }

        if (item == null || item.worldPrefab == null)
        {
            Debug.LogWarning("❌ Item chưa có prefab để spawn!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("⚠ SpawnPoint chưa được gán trong Inspector!");
            return;
        }

        // Spawn tại vị trí spawnPoint, làm con của player
        currentSpawned = Instantiate(item.worldPrefab, spawnPoint.position, spawnPoint.rotation, transform);

        // (Tuỳ chọn) reset local transform nếu muốn relative
        currentSpawned.transform.localRotation = Quaternion.identity;
        currentSpawned.transform.localScale = Vector3.one;
    }

    public void ClearItem()
    {
        if (currentSpawned != null)
        {
            Destroy(currentSpawned);
            currentSpawned = null;
        }
    }
}
