using UnityEngine;
using System.Collections.Generic;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ResourceSpawner : MonoBehaviour
{
    public static ResourceSpawner Instance { get; private set; }

    [Header("Spawn Data")]
    public ResourceData[] normalResources;   // dữ liệu resource thường
    public ResourceData[] rainyBonus;        // dữ liệu resource bonus

    [Header("Spawn Settings")]
    public Vector2 spawnAreaMin = new(-10f, -5f);  // góc dưới trái
    public Vector2 spawnAreaMax = new(10f, 5f);    // góc trên phải
    public int maxNormalCount = 10;
    public int maxRainyBonusCount = 5;

    // Danh sách object đã spawn
    private readonly List<GameObject> activeNormal = new();
    private readonly List<GameObject> activeRainy = new();

    private bool isSpawning = false;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void TryDailyRespawn()
    {
        if(isSpawning) return;
        isSpawning = true;
        SpawnBatch(normalResources, activeNormal, maxNormalCount);
        Debug.Log("🌍 ResourceSpawner: Đã kiểm tra spawn resource hàng ngày.");
        if (WeatherManager.Instance != null && WeatherManager.Instance.IsRaining())
            SpawnBatch(rainyBonus, activeRainy, maxRainyBonusCount);
    }
    private void SpawnBatch(ResourceData[] pool, List<GameObject> activeList, int maxCount)
    {
        if (pool == null || pool.Length == 0 || maxCount <= 0) return;

        int safety = 0; // tránh vòng lặp vô hạn
        while (activeList.Count < maxCount && safety < 1000)
        {
            safety++;

            var data = pool[Random.Range(0, pool.Length)];
            var pos = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                0f
            );

            GameObject obj = null;

            if (data.type == ResourceType.crop)
            {
                var planted = CropManager.Instance?.PlantCrop(pos, data.resourceName);
                if (planted != null)
                    obj = planted.gameObject;
            }
            else
            {
                obj = Instantiate(data.prefab, pos, Quaternion.identity);
            }

            if (obj != null)
            {
                activeList.Add(obj);
            }
            else
            {
                Debug.LogWarning($"⚠️ Spawn thất bại: {data.prefab.name} tại {pos}");
                break; // tránh infinite loop
            }
        }
    }

    public void ClearAll()
    {
        foreach (var obj in activeNormal) if (obj != null) Destroy(obj);
        foreach (var obj in activeRainy) if (obj != null) Destroy(obj);
        activeNormal.Clear();
        activeRainy.Clear();
    }

    // Giữ cho spawnAreaMin <= spawnAreaMax
    private void OnValidate()
    {
        if (spawnAreaMin.x > spawnAreaMax.x) (spawnAreaMin.x, spawnAreaMax.x) = (spawnAreaMax.x, spawnAreaMin.x);
        if (spawnAreaMin.y > spawnAreaMax.y) (spawnAreaMin.y, spawnAreaMax.y) = (spawnAreaMax.y, spawnAreaMin.y);
    }
    public void OnResourceDestroyed(GameObject obj,ResourceData data)
    {
        if(activeNormal.Contains(obj))activeNormal.Remove(obj);
        if(activeRainy.Contains(obj))activeRainy.Remove(obj);

        StartCoroutine(RespawnAfterDays(data));
    }
    private IEnumerator RespawnAfterDays(ResourceData data)
    {
        int today=TimeManager.Instance.GetAbsoluteDay();
        int respawnDay= today + Mathf.RoundToInt(data.respawnDays);
        while(TimeManager.Instance.GetAbsoluteDay()<respawnDay)
            yield return new WaitForSeconds(1f);

        var pos=new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            0f
        );
        Instantiate(data.prefab, pos, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 size2D = spawnAreaMax - spawnAreaMin;
        Vector3 size = new(size2D.x, size2D.y, 0f);
        Vector3 center = new(
            (spawnAreaMin.x + spawnAreaMax.x) * 0.5f,
            (spawnAreaMin.y + spawnAreaMax.y) * 0.5f,
            0f
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        Gizmos.DrawCube(center, size);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(new Vector3(spawnAreaMin.x, spawnAreaMin.y, 0f), 0.1f);
        Gizmos.DrawSphere(new Vector3(spawnAreaMax.x, spawnAreaMax.y, 0f), 0.1f);

#if UNITY_EDITOR
        Handles.Label(center, $"Spawn Area\nMin {spawnAreaMin}\nMax {spawnAreaMax}");
#endif
    }
}
