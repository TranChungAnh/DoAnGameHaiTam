using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
    [Header("Thông tin dữ liệu cây ")]
    public CropData data;
    public int plantedDay;
    public int currentStage = 0;
    public bool isDead = false;

    [Header("Sprite khi cây chết ")]
    public Sprite deadSprite;

    private SpriteRenderer sr;

    private void Start()
    {
        if (data != null && data.growthSprites.Length > 0)
        {
            UpdateStage(currentStage);
        }
    }

    private void Awake()
    {
        // Luôn gắn SpriteRenderer vào chính GameObject này
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
    }

    // Khởi tạo cây mới trồng 
    public void Init(CropData cropData, int today, Sprite deadspr)
    {
        data = cropData;
        plantedDay = today;
        deadSprite = deadspr;
        UpdateStage(0);
    }

    // Gọi mỗi ngày
    public void TickDaily(int today, Season curSeason, float soilMoisture)
    {
        if (isDead || data == null) return;

        int daysPassed = today - plantedDay;

        // kiểm tra tuổi thọ
        if (data.defaultLifetimeDays > 0 && daysPassed >= data.defaultLifetimeDays)
        {
            Kill();
            return;
        }

        // kiểm tra điều kiện phát triển
        bool seasonOk = System.Array.Exists(data.growSeasons, s => s == curSeason);

        if (!seasonOk ) return;

        // tính stage mới
        int accumulated = 0;
        int newStage = 0;
        for (int i = 0; i < data.growthDays.Length; i++)
        {
            accumulated += data.growthDays[i];
            if (daysPassed >= accumulated) newStage = i + 1;
        }

        if (newStage != currentStage) UpdateStage(newStage);
    }

    // Đổi sprite stage
    private void UpdateStage(int stage)
    {
        if (data.growthSprites != null && data.growthSprites.Length > stage)
        {
            sr.sprite = data.growthSprites[stage];
            Debug.Log($"🌱 Cây '{data.resourceName}' lên stage {stage}");
        }

        // Set Sorting Layer cho cây
        sr.sortingLayerName = "Plants"; // nhớ tạo layer "Plants" trong Project Settings > Tags & Layers
        sr.sortingOrder = -(int)(transform.position.y * 100); // auto sort theo Y

        currentStage = stage;
    }

    // Cây chết
    public void Kill()
    {
        if (isDead) return;
        isDead = true;

        if (sr != null && deadSprite != null)
        {
            sr.sprite = deadSprite;
        }

        Debug.Log($"🌳 Cây '{data.resourceName}' đã chết!");
    }

    private void LateUpdate()
    {
        // luôn cập nhật sorting order theo Y
        if (sr != null)
        {
            sr.sortingOrder = -(int)(transform.position.y * 100);
        }
    }
}
