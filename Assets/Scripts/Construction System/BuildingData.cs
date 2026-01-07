using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RecipeEntry
{
    public ItemBase item;
    public int amount;
}

[CreateAssetMenu(menuName = "Farming/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("Thông tin")]
    public string buildingName;
    public string buildingGroup; // ví dụ: "Fence"
    public GameObject[] directionnalPrefabs;
    public Sprite icon;
    public Vector2Int size = Vector2Int.one;  // kích thước chiếm lưới
    public bool rotatable = true;

    [Header("Công thức xây dựng")]
    public List<RecipeEntry> recipe = new List<RecipeEntry>();

    [Header("Preview")]
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

    [Header("Placement options")]
    public LayerMask blockingLayers;
    public Vector2 placementOffset = Vector2.zero;

    public bool autoConnect;
    public Sprite[] connectionSprites; // 0-15 pattern (binary mask)

}
