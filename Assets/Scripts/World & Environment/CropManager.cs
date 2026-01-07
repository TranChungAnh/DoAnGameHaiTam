using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance { get; private set; }

    [Header("Danh mục cây trồng")]
    public List<CropData> cropCatalog = new();  // danh sách loại cây (SO)

    [Header("Parent chứa tất cả cây trong scene")]
    public Transform cropParent;

    [Header("Sprite cây chết")]
    public Sprite deadSprite;

    [Header("Tham chiếu")]
    public Transform playerTransform;
    public Tilemap groundTilemap;

    // Danh sách cây đang trồng
    private readonly List<CropBehaviour> activeCrops = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public CropBehaviour PlantCrop(Vector3 worldPos, string cropName)
    {
        // Tìm dữ liệu cây theo tên
        var data = cropCatalog.Find(c => c.resourceName == cropName);
        if (data == null)
        {
            return null;
        }

        Vector3Int cell = groundTilemap.WorldToCell(worldPos);

        // Check có cây sẵn trong ô đất chưa
        if (HasCropAtCell(cell))
        {
            Debug.Log("⚠️ Ô đất này đã có cây!");
            return null;
        }

        // Tạo prefab
        var cropGO = Instantiate(data.prefab, groundTilemap.GetCellCenterWorld(cell), Quaternion.identity, cropParent);

        // Lấy CropBehaviour từ prefab
        var behaviour = cropGO.GetComponent<CropBehaviour>();
        if (behaviour == null)
        {
            behaviour = cropGO.AddComponent<CropBehaviour>();
        }

        // Khởi tạo
        behaviour.Init(data, TimeManager.Instance.GetAbsoluteDay(), deadSprite);

        // Lưu vào danh sách
        activeCrops.Add(behaviour);

        return behaviour;
    }

    // ====================================================
    // Trồng cây ngay trước mặt Player
    // ====================================================
    public CropBehaviour PlantCropInFrontOfPlayer(string cropName)
    {
        if (playerTransform == null || groundTilemap == null) return null;

        PlayerController pc = playerTransform.GetComponentInParent<PlayerController>();
        Vector2 dir = pc != null ? pc.FacingDirection : Vector2.down;

        Vector3Int playerCell = groundTilemap.WorldToCell(playerTransform.position);
        Vector3Int targetCell = playerCell + new Vector3Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), 0);
        Vector3 cellCenter = groundTilemap.GetCellCenterWorld(targetCell);

        return PlantCrop(cellCenter, cropName);
    }

    // ====================================================
    // Tick mỗi ngày → gọi sinh trưởng cho cây
    // ====================================================
    public void TickGrowthDaily()
    {
        int today = TimeManager.Instance.GetAbsoluteDay();
        Season curSeason = SeasonManager.Instance.CurrentSeason;

        foreach (var crop in activeCrops)
        {
            if (crop == null) continue;

            // Lấy moisture từ SoilManager
            SoilTile soil = SoilManager.Instance.GetTile((Vector2Int)groundTilemap.WorldToCell(crop.transform.position));
            float soilMoisture = soil != null ? soil.data.moisture : 0f;

            crop.TickDaily(today, curSeason, soilMoisture);
        }
    }

    // ====================================================
    // Khi đất khô thì cây chết
    // ====================================================
    public void OnSoilDryAt(Vector2Int pos)
    {
        foreach (var crop in activeCrops)
        {
            if (crop == null) continue;

            Vector3Int cell = groundTilemap.WorldToCell(crop.transform.position);
            if ((Vector2Int)cell == pos && !crop.isDead)
            {
                crop.Kill();
                break;
            }
        }
    }

    // ====================================================
    // Kiểm tra có cây ở cell
    // ====================================================
    public bool HasCropAtCell(Vector3Int cell)
    {
        foreach (var crop in activeCrops)
        {
            if (crop == null) continue;

            Vector3Int c = groundTilemap.WorldToCell(crop.transform.position);
            if (c == cell) return true;
        }
        return false;
    }
}
