using System.Collections.Generic;
using UnityEngine;

public enum AnimalStateType
{
    Hungry,          // Đói, cần cho ăn
    Thirsty,         // Khát, cần nước
    Sick,            // Ốm, cần chữa trị
    Dirty,           // Bẩn, cần vệ sinh
    Sleep,           // Đang ngủ
    Tired,           // Mệt, cần nghỉ ngơi
    Milk,            // Có sữa để thu hoạch
    Wool,            // Có lông/len để cắt
    Egg,             // Có trứng để thu hoạch
    Happy,           // Vui vẻ, hài lòng
    Sad,             // Buồn, không thoải mái
    Pregnant,        // Đang mang thai
    Growing,         // Đang lớn (chưa trưởng thành)
    ReadyForHarvest  // Đã sẵn sàng để lấy sản phẩm
}


[System.Serializable]
public class AnimalStateData
{
    public AnimalStateType stateType;

    [Header("Base Rate (sẽ bị random trong khoảng)")]
    public float rate = 30f;

    [Header("Khoảng random")]
    public float minRate = 20f;
    public float maxRate = 40f;

    public string cureItemName;
    public GameObject iconPrefab;
    public float surviveDuration = 30f;
}
public enum ProduceType
{
    Reproduce,   // Sinh con (vd: gà đẻ gà con, bò sinh bê)
    Harvest      // Thu hoạch sản phẩm (sữa, lông, trứng…)
}
[System.Serializable]
public class ProduceData
{
    public ProduceType type;
    public string produceName;
    public GameObject producePrefab;
    public ItemBase produceItem;

    [Header("Base Cooldown (sẽ bị random trong khoảng)")]
    public float cooldown = 30f;

    [Header("Khoảng random")]
    public float minCooldown = 20f;
    public float maxCooldown = 40f;

    public string requiredTool;
}

[CreateAssetMenu(fileName = "AnimalData", menuName = "FarmGame/AnimalData")]
public class AnimalData : ScriptableObject
{
    [Header("Stats")]
    public float moveSpeed = 1.5f;

    [Header("General Info")]
    public string animalName;

    [Header("Lifecycle")]
    public float minScale = 0.5f;   // scale lúc mới sinh
    public float maxScale = 1.5f;   // scale khi trưởng thành
    public float growDuration = 60f; // thời gian lớn (giây)
    public float lifeDuration = 300f; // tuổi thọ tổng

    [Header("Home Setting")]
    public string homeTag;
    [Header("States")]
    public List<AnimalStateData> states = new();

    [Header("Animation")]
    public RuntimeAnimatorController animatorController;

    [Header("Produce")]
    public List<ProduceData> produces = new();
}




