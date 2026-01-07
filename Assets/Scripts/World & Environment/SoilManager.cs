using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SoilManager : MonoBehaviour
{
    public static SoilManager Instance { get; private set; }

    [Header("Cấu hình grid đất")]
    public Tilemap groundTilemap;

    [Header("Tiles thay đổi trạng thái đất")]
    public TileBase normalSoilTile;
    public TileBase tilledSoilTile;
    public TileBase wateredSoilTile;

    // quản lý tất cả SoilTile
    public Dictionary<Vector2Int, SoilTile> tiles = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GenerateTiles();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        foreach (var (pos, tile) in tiles)
        {
            float before = tile.data.moisture;

            tile.UpdateMoisture(dt);

            // Nếu vừa khô (moisture từ >0 về 0)
            if (before > 0f && tile.data.moisture <= 0f && tile.HasCrop())
            {
                CropManager.Instance?.OnSoilDryAt(pos);
            }
        }
    }

    public List<SoilTile> GetAllSoilTiles() => tiles.Values.ToList();

    /// <summary>
    /// Quét tilemap ban đầu, tạo SoilTile cho từng ô đất
    /// </summary>
    private void GenerateTiles()
    {
        if (groundTilemap == null)
        {
            Debug.LogError("Ground Tilemap chưa được gán trong Inspector!");
            return;
        }

        BoundsInt bounds = groundTilemap.cellBounds;
        TileBase[] allTiles = groundTilemap.GetTilesBlock(bounds);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new(x, y, 0);
                if (groundTilemap.GetTile(tilePosition) == null) continue;

                Vector2Int pos = new(x, y);
                if (!tiles.ContainsKey(pos))
                {
                    tiles[pos] = new SoilTile(pos);
                }
            }
        }
    }

    // ==== TILE API ====

    public SoilTile GetTile(Vector2Int pos) =>
        tiles.TryGetValue(pos, out var tile) ? tile : null;

    public void WaterTile(Vector2Int pos)
    {
        if (!tiles.TryGetValue(pos, out var tile)) return;

        tile.Water();
        Debug.Log($"💧 Đã tưới ô {pos}, moisture = {tile.data.moisture}");
    }

    public void SetTileVisual(Vector2Int pos, bool isTilled, bool isWatered)
    {
        if (groundTilemap == null) return;

        Vector3Int tilePos = new(pos.x, pos.y, 0);
        TileBase tileBase = normalSoilTile;

        if (isWatered && wateredSoilTile != null) tileBase = wateredSoilTile;
        else if (isTilled && tilledSoilTile != null) tileBase = tilledSoilTile;

        groundTilemap.SetTile(tilePos, tileBase);
    }

    // ==== CHUYỂN NGÀY ====
    public void NewDay()
    {
        foreach (var tile in tiles.Values) tile.NewDay();

        // Nếu không mưa thì đất tự giảm moisture
        if (!WeatherManager.Instance.IsRaining())
        {
            foreach (var tile in tiles.Values)
            {
                tile.data.moisture = Mathf.Max(0f, tile.data.moisture - 0.2f);
                tile.data.isWatered = tile.data.moisture > 0f;
            }
        }
    }

    // ==== SỰ KIỆN KHÔ RIÊNG TỪNG Ô ====
    public void OnSoilDryAt(Vector2Int pos)
    {
        Debug.Log($"⚠️ Ô {pos} khô → cây trên ô này sẽ chết!");
        CropManager.Instance?.OnSoilDryAt(pos);
    }
}
