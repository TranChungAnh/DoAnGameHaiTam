using UnityEngine;

public enum ItemType
{
    Seed,      
    Tool,       
    Crop,      
    Material,   
    Other      
}

[CreateAssetMenu(menuName = "FarmGame/Item")]
public class ItemBase : ScriptableObject
{
    [Header("Thông tin chung")]
    public string itemName;
    public Sprite icon;
    [TextArea(2, 4)] public string description;
    public ItemType itemType ;
    [Header("Nếu là công cụ")]
    public ToolType toolType;


    [Header("Prefab trong game (dùng để spawn)")]
    public GameObject worldPrefab;
    [Header("Nếu là hạt giống")]
    public CropData cropData;
}
