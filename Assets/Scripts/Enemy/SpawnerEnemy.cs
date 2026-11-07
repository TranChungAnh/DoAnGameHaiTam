using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerEnemy : MonoBehaviour
{
    [Header("Enemy Prefabs Settings")]
    [SerializeField] private GameObject[] prefabsToSpawn; // Kéo các prefab Enemy con vào đây

    [Header("Spawn Area Settings")]
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(5f, 5f); // Vùng xuất hiện xung quanh Boss

    [Header("Minion Lifetime Settings")]
    [SerializeField] private float minionLifeTime = 5f; // Thời gian (giây) trước khi quái triệu hồi tự biến mất

    [Header("Cooldown Settings")]
    [SerializeField] private float coolDownTime = 2f; // Thời gian hồi chiêu triệu hồi (ví dụ 2 giây/lần)
    private float nextSpawnTime;

    private void Start()
    {
        // Trì hoãn lần gọi đầu tiên để khi vừa vào game Boss chưa triệu hồi ngay
        nextSpawnTime = Time.time + coolDownTime;
    }

    private void Update()
    {
        // Tự động kiểm tra thời gian, cứ hết cooldown là Boss triệu hồi đệ
        if (Time.time >= nextSpawnTime)
        {
            SpawnMinions();
            nextSpawnTime = Time.time + coolDownTime;
        }
    }

    // Hàm thực hiện triệu hồi Enemy
    public void SpawnMinions()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogWarning("Chưa gán Prefab Enemy nào cho Boss triệu hồi!");
            return;
        }

        foreach (GameObject prefab in prefabsToSpawn)
        {
            if (prefab != null)
            {
                // Tính toán vị trí ngẫu nhiên xung quanh vị trí của Boss
                float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
                float randomY = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
                Vector3 spawnPos = transform.position + new Vector3(randomX, randomY, 0f);

                // Triệu hồi Enemy con
                GameObject spawnedMinion = Instantiate(prefab, spawnPos, Quaternion.identity);

                // Tự động hủy (biến mất) con quái sau khoảng thời gian minionLifeTime
                Destroy(spawnedMinion, minionLifeTime);
            }
        }

    }

    // Vẽ khung vực vùng triệu hồi trong Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Đổi màu đỏ cho dễ phân biệt vùng triệu hồi đệ
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0f));
    }
}