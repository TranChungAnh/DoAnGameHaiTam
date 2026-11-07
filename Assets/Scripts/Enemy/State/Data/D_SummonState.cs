using UnityEngine;

[CreateAssetMenu(fileName = "newSummonStateData", menuName = "Data/State Data/Summon State")]
public class D_SummonState : ScriptableObject
{
    [Header("Tentacle Settings")]
    public GameObject tentaclePrefab;
    public float summonDuration = 1.5f;     // Thời gian State kéo dài
    public float summonCooldown = 10f;

    [Header("Spawning Range & Spacing")]
    public float spacing = 2.0f;
    public int numberOfTentacles = 5;
    public float spawnDelay = 0.15f;         // Tốc độ mọc lần lượt giữa các súc tua

    [Header("Optional Effects")]
    public bool randomOffset = true;
    public float offsetRange = 0.5f;
}