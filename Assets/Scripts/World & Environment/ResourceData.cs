using UnityEngine;

public enum ResourceType { crop,Tree, Rock, Ore, Bush }
[CreateAssetMenu(menuName = "Farming/Resource Data")]
public class ResourceData : ScriptableObject
{
    public string resourceName;
    public ResourceType type;
    public GameObject prefab;
    [Header("Thu hoạch ")]
    public int maxHealth = 3;
    public LootTable lootTable;
    public float respawnDays = 3;
}
