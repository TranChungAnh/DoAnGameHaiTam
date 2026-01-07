using Cinemachine.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;
    public InputAction mousePositionAction;

    [Header("Grid")]
    public float tileSize = 1f;
    public Vector2 gridOrigin = Vector2.zero;

    [Header("Preview settings")]
    public Material previewMaterial;
    public int previewSortingOrder = 100;


    private BuildingData currentBuilding;
    private GameObject previewObj;
    private SpriteRenderer previewRenderer;
    private bool isPlacing = false;
    private int currentRotation = 0;
    private Transform player;



    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        player = FindObjectOfType<PlayerController>()?.transform;
    }

    private void Update()
    {
        if (!isPlacing || currentBuilding == null) return;
        Vector3 snapped = Vector3.zero;
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            Vector2 dir = pc != null ? pc.FacingDirection : Vector2.down;

            Vector3Int playerCell = SoilManager.Instance.groundTilemap.WorldToCell(player.position);
            Vector3Int targetCell = playerCell + new Vector3Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), 0);
            Vector3 targetWorld = SoilManager.Instance.groundTilemap.GetCellCenterWorld(targetCell);

             snapped = SnapToGrid(targetWorld);
            snapped.z = 0f;

            previewObj.transform.position = snapped + (Vector3)currentBuilding.placementOffset;
            // cập nhật nối ghost 
            if(currentBuilding.autoConnect&& currentBuilding.connectionSprites != null)
            {
                var connect=previewObj.GetComponent<AutoConnectObject>();
                if (connect == null)
                { 
                    connect=previewObj.AddComponent<AutoConnectObject>();
                }
                connect.PreviewConnect(currentBuilding.buildingName, currentBuilding.buildingGroup, tileSize, currentBuilding.connectionSprites);

            }
        }
        //previewObj.transform.rotation = Quaternion.Euler(0, 0, currentRotation);

        bool hasResources = CheckResources(currentBuilding);
        bool canPlace = hasResources && !IsBlocked(snapped);

        if (previewRenderer != null)
            previewRenderer.color = canPlace ? currentBuilding.validColor : currentBuilding.invalidColor;

        if (IsPrimaryPressed() && canPlace)
        {
            foreach (var entry in currentBuilding.recipe)
                InventoryManager.Instance.RemoveItem(entry.item.itemName, entry.amount);

            //Quaternion rotation = Quaternion.Euler(0, 0, currentRotation);
            //GameObject obj = Instantiate(currentBuilding.prefab, snapped + (Vector3)currentBuilding.placementOffset, rotation);
            GameObject prefabToUse = currentBuilding.directionnalPrefabs[0];
            if (currentBuilding.directionnalPrefabs != null && currentBuilding.directionnalPrefabs.Length > 0)
            {
                int dirIndex = (currentRotation / 90) % currentBuilding.directionnalPrefabs.Length;
                prefabToUse = currentBuilding.directionnalPrefabs[dirIndex];

            }
            GameObject obj = Instantiate(prefabToUse, snapped + (Vector3)currentBuilding.placementOffset, Quaternion.identity);
            if (currentBuilding.autoConnect&& currentBuilding.connectionSprites != null)
            {
                var connect=obj.GetComponent<AutoConnectObject>();
                if (connect == null)
                {
                    connect=obj.AddComponent<AutoConnectObject>();
                }
                connect.Init(currentBuilding.buildingName, currentBuilding.buildingGroup, tileSize, currentBuilding.connectionSprites);
            }
            CancelPlacing();
        }

        if (IsSecondaryPressed())
            CancelPlacing();

        // Nhấn R để đổi ghost (không xoay prefab, mà thay prefab khác)
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame && currentBuilding.rotatable)
        {
            if (previewObj != null)
            {
                Destroy(previewObj);
            }
            previewRenderer = previewObj.GetComponentInChildren<SpriteRenderer>();

            // Tăng chỉ số prefab, quay vòng nếu hết
            int dirIndex = ((currentRotation / 90) + 1) % currentBuilding.directionnalPrefabs.Length;
            currentRotation = dirIndex * 90; // chỉ để lưu hướng hiện tại, không xoay ghost

            // Lấy đúng prefab theo hướng
            GameObject newPrefab = currentBuilding.directionnalPrefabs[dirIndex];

            // Tạo ghost mới từ prefab tương ứng
            previewObj = Instantiate(newPrefab);
            previewObj.name = "Preview_" + currentBuilding.buildingName + "_dir" + dirIndex;

            foreach (var col in previewObj.GetComponentsInChildren<Collider2D>())
                col.enabled = false;

            foreach (var sr in previewObj.GetComponentsInChildren<SpriteRenderer>())
            {
                Color c = sr.color;
                c.a = 0.5f;
                sr.color = c;
                sr.sortingOrder = previewSortingOrder;
                if (previewMaterial != null)
                    sr.material = previewMaterial;
            }

            var rb = previewObj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;

            var animator = previewObj.GetComponent<Animation>();
            if (animator != null) animator.enabled = false;
        }


    }

    public void StartPlacing(BuildingData building)
    {
        CancelPlacing();

        currentBuilding = building;
        isPlacing = true;
        currentRotation = 0;

        //previewObj = Instantiate(building.prefab);
        GameObject prefabToUse = building.directionnalPrefabs[0];
        if (building.directionnalPrefabs != null && building.directionnalPrefabs.Length > 0)
        {
            prefabToUse=building.directionnalPrefabs[0];
        }
        previewObj = Instantiate(prefabToUse);
        previewRenderer = previewObj.GetComponentInChildren<SpriteRenderer>();
        previewObj.name = "Preview_" + building.buildingName;

        foreach(var col in previewObj.GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }
        foreach(var sr in previewObj.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
            sr.sortingOrder = previewSortingOrder;
            if (previewMaterial != null)
            {
                sr.material = previewMaterial;
            }
        }
        var rb=previewObj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        var animator = previewObj.GetComponent<Animation>();
        if (animator != null) animator.enabled = false;
        Debug.Log($"Preview đã được tạo {building.buildingName}");

    }

    private void CancelPlacing()
    {
        isPlacing = false;
        currentBuilding = null;
        currentRotation = 0;
        if (previewObj != null) Destroy(previewObj);
    }

    //private Vector3 SnapToGrid(Vector3 worldPos)
    //{
    //    float worldX = (worldPos.x - gridOrigin.x) / tileSize;
    //    float worldY = (worldPos.y - gridOrigin.y) / tileSize;

    //    float snappedX = Mathf.Round(worldX) * tileSize + gridOrigin.x;
    //    float snappedY = Mathf.Round(worldY) * tileSize + gridOrigin.y;

    //    return new Vector3(snappedX, snappedY, 0f);
    //}
    private Vector3 SnapToGrid(Vector3 worldPos)
    {
        // Lấy cell tương ứng
        Vector3Int cell = SoilManager.Instance.groundTilemap.WorldToCell(worldPos);

        // Trả về tâm cell
        Vector3 center = SoilManager.Instance.groundTilemap.GetCellCenterWorld(cell);

        // Giữ Z = 0 để hiển thị 2D
        return new Vector3(center.x, center.y, 0f);
    }

    private bool CheckResources(BuildingData b)
    {
        foreach (var entry in b.recipe)
        {
            if (!InventoryManager.Instance.HasItem(entry.item.itemName, entry.amount))
                return false;
        }
        return true;
    }

    private bool IsBlocked(Vector3 centerWorld)
    {
        Vector2 sizeWorld = new Vector2(currentBuilding.size.x * tileSize, currentBuilding.size.y * tileSize);

        if (currentRotation == 90 || currentRotation == 270)
            sizeWorld = new Vector2(sizeWorld.y, sizeWorld.x);

        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)centerWorld + currentBuilding.placementOffset,
                                                    sizeWorld,
                                                    currentRotation,
                                                    currentBuilding.blockingLayers);
        return hits != null && hits.Length > 0;
    }

    private Vector3 GetWorldPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 screen = new Vector3(touchPos.x, touchPos.y, Mathf.Abs(Camera.main.transform.position.z));
            return Camera.main.ScreenToWorldPoint(screen);
        }
        else if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 screen = new Vector3(mousePos.x, mousePos.y, Mathf.Abs(Camera.main.transform.position.z));
            return Camera.main.ScreenToWorldPoint(screen);
        }
        return Vector3.zero;

    }

    private bool IsPrimaryPressed()
    {
        return (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
    }

    private bool IsSecondaryPressed()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            return true;

        if (Touchscreen.current != null)
        {
            int count = 0;
            foreach (var t in Touchscreen.current.touches)
                if (t.press.isPressed) count++;
            return count >= 2;
        }
        return false;
    }


}