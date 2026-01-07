using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "NewCrop", menuName = "Farming/Crop Data")]
public class CropData : ResourceData
{
    [Header("Sprite tăng trưởng")]
    public Sprite[] growthSprites;     // sprite theo stage
    public int[] growthDays;           // số ngày cho mỗi stage

    [Header("Điều kiện phát triển")]
    public Season[] growSeasons;       // mùa trồng hợp lệ
    public bool needsWater = true;

    [Header("Thu hoạch")]
    public Item harvestItem;
    public int harvestAmount = 1;
    public bool regrowAfterHarvest = false; // tái sinh (vd: cà chua)

    [Header("Thời gian sống")]
    [Tooltip("-1 nghĩa là sống vô hạn, >0 nghĩa là số ngày tối đa trước khi chết.")]
    public float defaultLifetimeDays = -1f;
}
