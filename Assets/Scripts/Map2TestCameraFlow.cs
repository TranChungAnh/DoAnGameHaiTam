#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;


public class HaiTamMap2BuilderV7 : EditorWindow
{
    // ============================================================
    // CẤU HÌNH
    // ============================================================
    const int W = 120;
    const int H = 82;
    const int PPU = 16;
    const string SCENE_PATH = "Assets/Scenes/Map2_RungVong_V7.unity";
    const string ROOT_NAME = "MAP2_V7";

    // Thư mục chứa Tile/PNG của bạn. FindAssets vẫn quét toàn project.
    const string DEFAULT_ASSET_FOLDER = "Assets/GeneratedMap2Art";

    // Tên tile ưu tiên. Nếu project có tile với tên này, builder sẽ tự tìm.
    const string GRASS = "Grass1";
    const string GRASS_DARK = "GrassDark1";
    const string DIRT = "Dirt";
    const string DIRT_LIGHT = "DirtLight";
    const string WATER = "Water1";
    const string WATER_LIGHT = "WaterLight";
    const string SHORE = "Shore";
    const string ROCK = "Rock";
    const string ROCK_DARK = "RockDark";
    const string WOOD = "WoodBridge";

    // ===== V7: Rotate map 90° clockwise =====
    static Vector3Int Rot(int x, int y)
    {
        return new Vector3Int(y, W - 1 - x, 0);
    }

    static Vector3 RotWorld(int x, int y)
    {
        Vector3Int p = Rot(x, y);
        return new Vector3(p.x + 0.5f, p.y + 0.5f, 0);
    }

    [MenuItem("Tools/Hai Tam Map/BUILD MAP 2 V7 - TILEMAP")]
    public static void Build()
    {
        EnsureFolders();
        AssetDatabase.Refresh();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        EnsureSortingLayers();

        GameObject root = new GameObject(ROOT_NAME);
        Grid grid = root.AddComponent<Grid>();
        grid.cellSize = Vector3.one;

        Tilemap ground = CreateTilemap(root, "Ground", "Ground", false);
        Tilemap path = CreateTilemap(root, "Path", "GroundDetail", false);
        Tilemap water = CreateTilemap(root, "Water", "Ground", false);
        Tilemap structures = CreateTilemap(root, "Structures", "Structures", true);
        Tilemap decor = CreateTilemap(root, "Decor", "Overhead", false);

        TileBase grass = FindOrCreateTile(GRASS);
        TileBase grassDark = FindOrCreateTile(GRASS_DARK);
        TileBase dirt = FindOrCreateTile(DIRT);
        TileBase dirtLight = FindOrCreateTile(DIRT_LIGHT);
        TileBase waterTile = FindOrCreateTile(WATER);
        TileBase waterLight = FindOrCreateTile(WATER_LIGHT);
        TileBase shore = FindOrCreateTile(SHORE);
        TileBase rock = FindOrCreateTile(ROCK);
        TileBase rockDark = FindOrCreateTile(ROCK_DARK);
        TileBase wood = FindOrCreateTile(WOOD);

        if (grass == null || dirt == null)
        {
            EditorUtility.DisplayDialog(
                "Map 2 V7",
                "Không tìm thấy tile đất.\n\n" +
                "Hãy đặt PNG/Tile đất vào Assets/GeneratedMap2Art hoặc đổi DEFAULT_ASSET_FOLDER.\n" +
                "Tên ưu tiên: Grass1 và Dirt.",
                "OK");
            return;
        }

        // 1) TẠO MẶT ĐẤT TỪNG Ô - KHÔNG FillNoisy hình chữ nhật.
        bool[,] land = BuildForestLandMask();
        PaintLand(ground, land, grass, grassDark);

        // 2) BIỂN/VIỀN NƯỚC theo mask.
        PaintWaterAroundLand(water, ground, land, waterTile, waterLight, shore);

        // 3) ĐƯỜNG CHÍNH + NHÁNH - từng ô, liên tục.
        List<Vector2Int> mainPath = BuildMainPath();
        PaintPath(path, mainPath, dirt, dirtLight, 1);

        List<Vector2Int> branchA = BuildPath(new[]
        {
            new Vector2Int(58, 27), new Vector2Int(49, 23),
            new Vector2Int(43, 18), new Vector2Int(43, 15)
        });
        List<Vector2Int> branchB = BuildPath(new[]
        {
            new Vector2Int(54, 35), new Vector2Int(45, 37),
            new Vector2Int(38, 40), new Vector2Int(33, 43)
        });
        List<Vector2Int> branchC = BuildPath(new[]
        {
            new Vector2Int(58, 49), new Vector2Int(68, 48),
            new Vector2Int(76, 45), new Vector2Int(86, 42),
            new Vector2Int(93, 43)
        });

        PaintPath(path, branchA, dirt, dirtLight, 1);
        PaintPath(path, branchB, dirt, dirtLight, 1);
        PaintPath(path, branchC, dirt, dirtLight, 1);

        // 4) CỔNG: chỉ giữ vùng đất + đường nối. Cổng thật của bạn đặt ở đây.
        PaintPath(path, BuildPath(new[]
        {
            new Vector2Int(59, 9), new Vector2Int(59, 13),
            new Vector2Int(58, 18)
        }), dirtLight, dirt, 1);

        // 5) HỒ NƯỚC - tạo bằng mask, không phải một hình chữ nhật nước.
        PaintPond(water, ground, path, waterTile, waterLight, shore);

        // 6) TRẠM NGHIÊN CỨU 03 - nền/collider khung đơn giản.
        BuildResearchStation(structures, path, rock, rockDark, dirtLight);

        // 7) LỐI RA MAP 3.
        BuildMap3Exit(structures, path, rock, rockDark, wood);

        // 8) MARKER GAMEPLAY.
        GameObject markers = new GameObject("Gameplay_Markers");
        CreateMarker(markers.transform, "SpawnPoint_Map2", Cell(59, 9));

        CreateMarker(markers.transform, "Marker_ChildhoodMemory_OldCamp", Cell(56, 8));
        CreateMarker(markers.transform, "Marker_NPC_VoiceGuide_MemoryEcho", Cell(59, 12));

        CreateMarker(markers.transform, "Marker_ResearchGuard", Cell(74, 33));
        CreateMarker(markers.transform, "Marker_ResearchGate", Cell(55, 26));
        CreateMarker(markers.transform, "Marker_DigitOrderMap", Cell(56, 25));
        CreateMarker(markers.transform, "Marker_MachineRoom_FatherSighting", Cell(62, 21));
        CreateMarker(markers.transform, "Marker_ControlPanel_6Switches", Cell(55, 21));
        CreateMarker(markers.transform, "Marker_LabDoor", Cell(55, 20));
        CreateMarker(markers.transform, "Marker_ResearchLog_BotMongThuc", Cell(55, 19));
        CreateMarker(markers.transform, "Marker_Artifact_03", Cell(55, 18));
        CreateMarker(markers.transform, "Marker_FixedDiThu_Artifact04Holder", Cell(18, 33));
        CreateMarker(markers.transform, "Marker_Artifact_04", Cell(18, 32));
        CreateMarker(markers.transform, "Marker_CaveDoor", Cell(93, 43));
        CreateMarker(markers.transform, "Marker_ExitToMap3", Cell(93, 44));
        CreateMarker(markers.transform, "Marker_LakeBridge", Cell(37, 40));

        CreateMarker(markers.transform, "EnemySpawn_DiThu_01", Cell(15, 20));
        CreateMarker(markers.transform, "EnemySpawn_DiThu_02", Cell(28, 40));
        CreateMarker(markers.transform, "EnemySpawn_DiThu_03", Cell(13, 55));
        CreateMarker(markers.transform, "EnemySpawn_DiThu_04", Cell(100, 62));
        CreateMarker(markers.transform, "EnemySpawn_VatThiNghiem_01", Cell(45, 24));
        CreateMarker(markers.transform, "EnemySpawn_VatThiNghiem_02", Cell(64, 24));
        CreateMarker(markers.transform, "EnemySpawn_VatThiNghiem_03", Cell(58, 30));

        CreateMarker(markers.transform, "Clue_Number_01", Cell(32, 24));
        CreateMarker(markers.transform, "Clue_Number_02", Cell(27, 54));
        CreateMarker(markers.transform, "Clue_Number_03", Cell(36, 18));
        CreateMarker(markers.transform, "Clue_Number_04", Cell(72, 34));
        CreateMarker(markers.transform, "Clue_Number_05", Cell(67, 55));
        CreateMarker(markers.transform, "Clue_Number_06", Cell(78, 23));

        // 9) CÁC CỤM CÂY/ĐÁ CHỈ LÀ DECOR - không thay đổi topology của nền.
        ScatterDecor(decor.transform, land, path);

        // 10) Cổng và các prefab thật của bạn sẽ đặt trên marker SpawnPoint_Map2.
        // Không tạo cổng giả bằng khối tile.

        EditorSceneManager.SaveScene(scene, SCENE_PATH);
        Selection.activeGameObject = root;
        SceneView.lastActiveSceneView?.FrameSelected();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Hiển thị camera vào giữa map
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 45f;
            cam.transform.position = new Vector3(H * 0.5f, W * 0.5f, -10f);
        }

        // Lưu scene
        bool ok = EditorSceneManager.SaveScene(scene, SCENE_PATH);

        Selection.activeGameObject = ground.gameObject;
        SceneView.lastActiveSceneView?.FrameSelected();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Map2 V7 SaveScene = {ok} -> {SCENE_PATH}");

        EditorUtility.DisplayDialog(
            "Map 2 V7",
            ok
                ? $"Đã tạo scene:\n{SCENE_PATH}"
                : "SaveScene thất bại. Kiểm tra Console.",
            "OK");

       
    }

    // ============================================================
    // LAND MASK - hình khu rừng không vuông vức
    // ============================================================
    static bool[,] BuildForestLandMask()
    {
        bool[,] m = new bool[W, H];

        // Các "blob" chồng lên nhau tạo silhouette tự nhiên.
        AddEllipse(m, 58, 40, 50, 34);
        AddEllipse(m, 33, 25, 30, 25);
        AddEllipse(m, 88, 25, 31, 24);
        AddEllipse(m, 31, 58, 31, 22);
        AddEllipse(m, 88, 58, 32, 23);

        // Xóa một số góc để tránh cảm giác hình chữ nhật.
        CarveEllipse(m, 4, 4, 17, 16);
        CarveEllipse(m, 112, 4, 16, 17);
        CarveEllipse(m, 4, 67, 18, 14);
        CarveEllipse(m, 108, 67, 18, 14);

        // Mở rộng/làm tự nhiên các khu vực nối.
        AddEllipse(m, 58, 17, 26, 18);
        AddEllipse(m, 57, 65, 30, 19);

        // Bảo đảm corridor cho đường chính.
        foreach (Vector2Int p in BuildMainPath())
            StampCircle(m, p.x, p.y, 3);

        // Cổng và trạm phải nằm trên đất.
        StampCircle(m, 59, 9, 5);
        StampRect(m, 41, 15, 29, 15);
        StampRect(m, 80, 34, 33, 21);

        return m;
    }

    static void AddEllipse(bool[,] m, int cx, int cy, int rx, int ry)
    {
        for (int x = cx - rx; x <= cx + rx; x++)
            for (int y = cy - ry; y <= cy + ry; y++)
            {
                if (!Inside(x, y)) continue;
                float dx = (x - cx) / (float)rx;
                float dy = (y - cy) / (float)ry;
                if (dx * dx + dy * dy <= 1f)
                    m[x, y] = true;
            }
    }

    static void CarveEllipse(bool[,] m, int cx, int cy, int rx, int ry)
    {
        for (int x = cx - rx; x <= cx + rx; x++)
            for (int y = cy - ry; y <= cy + ry; y++)
            {
                if (!Inside(x, y)) continue;
                float dx = (x - cx) / (float)Mathf.Max(1, rx);
                float dy = (y - cy) / (float)Mathf.Max(1, ry);
                if (dx * dx + dy * dy <= 1f)
                    m[x, y] = false;
            }
    }

    static void StampCircle(bool[,] m, int cx, int cy, int r)
    {
        for (int x = cx - r; x <= cx + r; x++)
            for (int y = cy - r; y <= cy + r; y++)
                if (Inside(x, y) && (x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r)
                    m[x, y] = true;
    }

    static void StampRect(bool[,] m, int x0, int y0, int w, int h)
    {
        for (int x = x0; x < x0 + w; x++)
            for (int y = y0; y < y0 + h; y++)
                if (Inside(x, y)) m[x, y] = true;
    }

    // ============================================================
    // PAINT LAND / WATER
    // ============================================================
    static void PaintLand(Tilemap tm, bool[,] land, TileBase grass, TileBase dark)
    {
        // Nếu project có bộ Grass_00..Grass_15, tự chọn theo 4-neighbor.
        // Nếu không có, dùng Grass1/GrassDark1 như fallback.
        TileBase[] autoGrass = LoadAutoVariants("Grass");
        TileBase[] autoDark = LoadAutoVariants("GrassDark");

        for (int x = 0; x < W; x++)
            for (int y = 0; y < H; y++)
            {
                if (!land[x, y]) continue;

                int mask = LandMask(land, x, y);
                TileBase tile = null;

                if (autoGrass != null && autoGrass.Length == 16 && autoGrass[mask] != null)
                    tile = autoGrass[mask];
                else if (dark != null && CountLandNeighbors(land, x, y) <= 2)
                    tile = dark;
                else
                    tile = grass;

                tm.SetTile(Rot(x, y), tile);
            }
    }

    static int LandMask(bool[,] land, int x, int y)
    {
        int mask = 0;
        if (Inside(x, y + 1) && land[x, y + 1]) mask |= 1; // N
        if (Inside(x + 1, y) && land[x + 1, y]) mask |= 2; // E
        if (Inside(x, y - 1) && land[x, y - 1]) mask |= 4; // S
        if (Inside(x - 1, y) && land[x - 1, y]) mask |= 8; // W
        return mask;
    }

    static TileBase[] LoadAutoVariants(string prefix)
    {
        TileBase[] result = new TileBase[16];
        int found = 0;

        for (int i = 0; i < 16; i++)
        {
            string[] names =
            {
                prefix + "_" + i.ToString("00"),
                prefix + (i + 1).ToString("00"),
                prefix + "_" + (i + 1).ToString("00")
            };

            TileBase t = null;
            foreach (string name in names)
            {
                t = FindOrCreateTile(name);
                if (t != null) break;
            }

            result[i] = t;
            if (t != null) found++;
        }

        return found == 16 ? result : null;
    }

    static void PaintWaterAroundLand(Tilemap water, Tilemap ground, bool[,] land,
        TileBase waterTile, TileBase waterLight, TileBase shore)
    {
        if (waterTile == null) return;

        for (int x = 0; x < W; x++)
            for (int y = 0; y < H; y++)
            {
                if (land[x, y]) continue;

                bool near = false;
                for (int dx = -1; dx <= 1 && !near; dx++)
                    for (int dy = -1; dy <= 1 && !near; dy++)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (Inside(nx, ny) && land[nx, ny]) near = true;
                    }

                if (near && shore != null)
                    ground.SetTile(Rot(x, y), shore);

                water.SetTile(Rot(x, y),
                    waterLight != null && ((x + y) % 7 == 0) ? waterLight : waterTile);
            }
    }

    // ============================================================
    // PATH - từng ô, không phải Fill rectangle
    // ============================================================
    static List<Vector2Int> BuildMainPath()
    {
        return BuildPath(new[]
        {
            new Vector2Int(10,41),
            new Vector2Int(22,42),
            new Vector2Int(34,40),
            new Vector2Int(47,38),
            new Vector2Int(60,36),
            new Vector2Int(73,35),
            new Vector2Int(86,34),
            new Vector2Int(98,33)
        });
    }

    static List<Vector2Int> BuildPath(Vector2Int[] control)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        if (control == null || control.Length == 0) return result;

        for (int i = 0; i < control.Length - 1; i++)
        {
            Vector2Int a = control[i];
            Vector2Int b = control[i + 1];

            int steps = Mathf.Max(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y));
            for (int s = 0; s <= steps; s++)
            {
                float t = steps == 0 ? 0f : s / (float)steps;
                Vector2Int p = new Vector2Int(
                    Mathf.RoundToInt(Mathf.Lerp(a.x, b.x, t)),
                    Mathf.RoundToInt(Mathf.Lerp(a.y, b.y, t))
                );

                if (!result.Contains(p))
                    result.Add(p);
            }
        }

        return result;
    }

    static void PaintPath(Tilemap tm, List<Vector2Int> cells,
        TileBase main, TileBase accent, int radius)
    {
        if (main == null) return;

        foreach (Vector2Int p in cells)
        {
            for (int dx = -radius; dx <= radius; dx++)
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (dx * dx + dy * dy > radius * radius) continue;

                    Vector3Int cell = new Vector3Int(p.x + dx, p.y + dy, 0);
                    TileBase chosen = (accent != null && ((cell.x + cell.y) % 9 == 0))
                        ? accent : main;

                    tm.SetTile(cell, chosen);
                }
        }
    }

    // ============================================================
    // POND
    // ============================================================
    static void PaintPond(Tilemap water, Tilemap ground, Tilemap path,
        TileBase waterTile, TileBase waterLight, TileBase shore)
    {
        int cx = 37, cy = 40, rx = 10, ry = 8;

        for (int x = cx - rx - 2; x <= cx + rx + 2; x++)
            for (int y = cy - ry - 2; y <= cy + ry + 2; y++)
            {
                if (!Inside(x, y)) continue;

                float dx = (x - cx) / (float)rx;
                float dy = (y - cy) / (float)ry;
                float d = dx * dx + dy * dy;

                if (d <= 1f)
                {
                    // Không đè lên đường/cầu.
                    if (path.GetTile(new Vector3Int(x, y, 0)) != null) continue;

                    water.SetTile(Rot(x, y),
                        waterLight != null && ((x * 3 + y) % 11 == 0) ? waterLight : waterTile);
                    ground.SetTile(Rot(x, y), null);
                }
                else if (d <= 1.35f && shore != null)
                {
                    ground.SetTile(Rot(x, y), shore);
                }
            }

        if (FindOrCreateTile(WOOD) != null)
        {
            TileBase wood = FindOrCreateTile(WOOD);
            for (int x = 34; x <= 40; x++)
            {
                int y = 40;
                water.SetTile(Rot(x, y), null);
                ground.SetTile(Rot(x, y), wood);
            }
        }
    }

    // ============================================================
    // RESEARCH STATION / EXIT
    // ============================================================
    static void BuildResearchStation(Tilemap structures, Tilemap path,
        TileBase rock, TileBase rockDark, TileBase dirtLight)
    {
        if (rockDark == null) return;

        // Sân trước trạm.
        if (dirtLight != null)
            PaintPath(path, BuildPath(new[]
            {
                new Vector2Int(55, 26), new Vector2Int(55, 21),
                new Vector2Int(55, 16), new Vector2Int(55, 12)
            }), dirtLight, null, 1);

        // Viền công trình theo từng ô.
        for (int x = 44; x < 68; x++)
        {
            structures.SetTile(Rot(x, 16), rockDark);
            structures.SetTile(Rot(x, 27), rockDark);
        }

        for (int y = 16; y <= 27; y++)
        {
            structures.SetTile(Rot(43, y), rockDark);
            structures.SetTile(Rot(68, y), rockDark);
        }

        // Khoảng cửa.
        for (int x = 53; x <= 58; x++)
            structures.SetTile(Rot(x, 27), null);

        // Nội thất gợi ý bằng block, sau này thay bằng prefab trạm thật.
        if (rock != null)
        {
            PaintBlock(structures, rock, 46, 18, 65, 25);
            PaintBlock(structures, rockDark, 48, 20, 51, 22);
            PaintBlock(structures, rockDark, 60, 20, 63, 22);
            PaintBlock(structures, rockDark, 61, 18, 64, 20);
            PaintBlock(structures, rockDark, 61, 22, 64, 24);

            for (int x = 53; x <= 58; x++)
                for (int y = 25; y <= 27; y++)
                    structures.SetTile(Rot(x, y), null);
        }
    }

    static void BuildMap3Exit(Tilemap structures, Tilemap path,
        TileBase rock, TileBase rockDark, TileBase wood)
    {
        if (rockDark == null) return;

        PaintBlock(structures, rockDark, 81, 34, 111, 53);
        if (rock != null)
            PaintBlock(structures, rock, 84, 36, 108, 51);

        // Khoét cửa hang.
        for (int x = 89; x <= 97; x++)
            for (int y = 39; y <= 47; y++)
                structures.SetTile(Rot(x, y), null);

        if (path != null)
            PaintPath(path, BuildPath(new[]
            {
                new Vector2Int(83, 39), new Vector2Int(90, 42),
                new Vector2Int(93, 44)
            }), FindOrCreateTile(DIRT_LIGHT), null, 1);

        if (wood != null)
            PaintBlock(structures, wood, 90, 42, 96, 46);
    }

    static void PaintBlock(Tilemap tm, TileBase tile, int x0, int y0, int x1, int y1)
    {
        if (tm == null || tile == null) return;
        for (int x = x0; x <= x1; x++)
            for (int y = y0; y <= y1; y++)
                tm.SetTile(Rot(x, y), tile);
    }

    // ============================================================
    // DECOR
    // ============================================================
    static void ScatterDecor(Transform parent, bool[,] land, Tilemap path)
    {
        GameObject deco = new GameObject("Decor_Slots");
        deco.transform.SetParent(parent);

        int[,] spots =
        {
            {10,15},{20,10},{30,14},{14,25},{21,29},
            {11,38},{17,49},{12,61},{25,67},
            {74,12},{85,10},{100,14},{75,26},{103,28},
            {76,58},{100,60},{82,70},{103,71}
        };

        for (int i = 0; i < spots.GetLength(0); i++)
        {
            int x = spots[i, 0], y = spots[i, 1];
            if (!Inside(x, y) || !land[x, y]) continue;
            if (path.GetTile(new Vector3Int(x, y, 0)) != null) continue;

            CreateMarker(deco.transform, "ForestCluster_" + i, Cell(x, y));
        }

        // Các điểm này là slot để bạn kéo prefab cây/đá thật vào.
        // Không tự sinh sprite giả nữa.
    }

    // ============================================================
    // TILE LOADER - tìm Tile asset hoặc Sprite trong project
    // ============================================================
    static TileBase FindOrCreateTile(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        string[] tileGuids = AssetDatabase.FindAssets(name + " t:Tile");
        foreach (string guid in tileGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
            if (tile != null) return tile;
        }

        string[] spriteGuids = AssetDatabase.FindAssets(name + " t:Sprite");
        foreach (string guid in spriteGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null) continue;

            ConfigureSprite(path);

            string tilePath = "Assets/GeneratedMap2Art/Tile_" + name + ".asset";
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);

            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = sprite;
                AssetDatabase.CreateAsset(tile, tilePath);
            }
            else
            {
                tile.sprite = sprite;
                EditorUtility.SetDirty(tile);
            }

            return tile;
        }

        return null;
    }

    static Tilemap CreateTilemap(GameObject parent, string name, string sortingLayer, bool collider)
    {
        GameObject go = new GameObject("Tilemap_" + name);
        go.transform.SetParent(parent.transform, false);

        Tilemap tm = go.AddComponent<Tilemap>();
        TilemapRenderer tr = go.AddComponent<TilemapRenderer>();
        tr.sortingLayerName = sortingLayer;

        if (collider)
        {
            TilemapCollider2D tc = go.AddComponent<TilemapCollider2D>();
            tc.usedByComposite = true;

            CompositeCollider2D cc = go.AddComponent<CompositeCollider2D>();
            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            rb.simulated = true;
        }

        return tm;
    }

    // ============================================================
    // MARKER / UTILS
    // ============================================================
    static GameObject CreateMarker(Transform parent, string name, Vector3 position)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        return go;
    }

    static Vector3 Cell(int x, int y) => RotWorld(x, y);

    static int CountLandNeighbors(bool[,] land, int x, int y)
    {
        int n = 0;
        if (Inside(x + 1, y) && land[x + 1, y]) n++;
        if (Inside(x - 1, y) && land[x - 1, y]) n++;
        if (Inside(x, y + 1) && land[x, y + 1]) n++;
        if (Inside(x, y - 1) && land[x, y - 1]) n++;
        return n;
    }

    static bool Inside(int x, int y) => x >= 0 && x < W && y >= 0 && y < H;

    static void ConfigureSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.spritePixelsPerUnit = PPU;
        importer.wrapMode = TextureWrapMode.Clamp;

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        if (!AssetDatabase.IsValidFolder(DEFAULT_ASSET_FOLDER))
            AssetDatabase.CreateFolder("Assets", "GeneratedMap2Art");
    }

    static void EnsureSortingLayers()
    {
        SerializedObject tagManager =
            new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("m_SortingLayers");

        string[] names =
        {
            "Ground", "GroundDetail", "Structures", "Interactables",
            "Characters", "Overhead", "Effects", "UI"
        };

        foreach (string wanted in names)
        {
            bool found = false;

            for (int i = 0; i < layers.arraySize; i++)
            {
                SerializedProperty e = layers.GetArrayElementAtIndex(i);
                SerializedProperty n = e.FindPropertyRelative("name");

                if (n != null && n.stringValue == wanted)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                layers.InsertArrayElementAtIndex(layers.arraySize);

                SerializedProperty e =
                    layers.GetArrayElementAtIndex(layers.arraySize - 1);

                e.FindPropertyRelative("name").stringValue = wanted;
                e.FindPropertyRelative("uniqueID").intValue =
                    Guid.NewGuid().GetHashCode();
            }
        }

        tagManager.ApplyModifiedProperties();
    }
}
#endif
