using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ToolManager : MonoBehaviour
{
    private Camera cam;

    [Header("Hạt giống mặc định khi trồng")]
    [SerializeField] private CropData defaultSeed;   // Hạt giống mặc định
    private CropData currentSeed;                     // Hạt giống đang chọn
    private PlayerAnimation playerAnim;
    private void Awake()
    {
        cam = Camera.main;
        currentSeed = defaultSeed; // Khởi tạo seed mặc định
        playerAnim=FindObjectOfType<PlayerAnimation>();
    }

    // ---------------- SỰ KIỆN INPUT ----------------

    public void OnUseTool(InputAction.CallbackContext context)
    {
        Debug.Log("Đang sử dụng công cụ...");
        if (!context.performed) return;

        var selectedItem = HotbarManager.Instance.SselectedItem;
        if (selectedItem == null || selectedItem.itemType != ItemType.Tool)
        {
            Debug.Log("⛔ Không cầm công cụ nào");
            return;
        }

        ToolType currentTool = selectedItem.toolType;

        if (currentTool == ToolType.Hoe)
        {
            var tile = GetTargetSoilTile();
            if (tile != null)
            {
                tile.Till();
                playerAnim?.PlayerActionAnimation("Hoe");
            }
        }
        else
        {
            UseToolOnResource(currentTool);
            playerAnim?.PlayerActionAnimation(currentTool.ToString());
            Debug.Log($"Đang sử dụng công cụ: {currentTool}");
        }
    }


    public void OnPlantSeed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlantSeed();
            Debug.Log("Đang trồng hạt giống...");
            //playerAnim?.PlayerActionAnimation("Plant");
        }
    }

    public void OnWater(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Water();
            //playerAnim?.PlayerActionAnimation("Water");

        }
    }
   
    public void OnPoint(InputAction.CallbackContext context)
    {
        Vector2 screenPos = context.ReadValue<Vector2>();
    }

    // ---------------- LOGIC DỤNG CỤ ----------------

    //private void UseTool()
    //{
    //    var tile = GetTargetSoilTile();
    //    if (tile != null)
    //    {
    //        tile.Till();
    //    }
    //}

    private void PlantSeed()
    {
        //var tile = GetTargetSoilTile();
        //if (tile == null || !tile.data.isTilled) return;
        var pos = GetFrontCellWrldPos();
        if (pos == null) return;
        Vector3Int cell = SoilManager.Instance.groundTilemap.WorldToCell(pos.Value);
        SoilTile tile = SoilManager.Instance.GetTile(new Vector2Int(cell.x, cell.y));
        if (tile == null || !tile.data.isTilled) return;

        var selectedItem = HotbarManager.Instance.SselectedItem;
        if (selectedItem == null || selectedItem.itemType != ItemType.Seed || selectedItem.cropData == null) return;

        var seedData = selectedItem.cropData;

        // Ưu tiên hotbar
        var uiSlot = HotbarManager.Instance.hotbarSlots[HotbarManager.Instance.selectedIndex];
        var slot = uiSlot.boundSlot;

        if (slot != null && !slot.IsEmpty && slot.item == selectedItem)
        {
            if (slot.quantity > 0)
            {
                // 👉 chỉ trừ khi trồng thành công
                if (tile.PlantCrop(seedData))
                {
                    slot.quantity--;
                    if (slot.quantity <= 0) slot.Clear();
                    uiSlot.Refresh();
                }
                return;
            }
        }

        // Check inventory
        var storage = InventoryManager.Instance.playerInventory;
        if (storage == null) return;

        if (storage.HasItem(selectedItem.itemName, 1))
        {
            if (tile.PlantCrop(seedData))
            {
                storage.RemoveItem(selectedItem.itemName, 1);
            }
        }
    }


    private void Water()
    {
        var tile = GetTargetSoilTile();
        if (tile != null)
        {
            tile.Water();
        }
    }
    private void UseToolOnResource(ToolType tool)
    {
        var pos = GetFrontCellWrldPos();
        if (pos == null) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos.Value, 0.3f);
        foreach (var hit in hits)
        {
            var res = hit.GetComponent<ResourceBehaviour>();
            if (res != null)
            {
                if (ToolHelper.IsValidTool(res.data.type, tool))
                {
                    res.TakeDamage(1, tool);
                    Debug.Log($"Đã sử dụng {tool} lên {res.data.resourceName}");
                }
                else
                {
                    Debug.Log($"⛔ Không thể sử dụng {tool} lên {res.data.resourceName}");
                }
                return;
            }
        }
    }
    // ---------------- LẤY TILE THEO CHUỘT ----------------
    private Vector2Int? lastGridPos; // Lưu vị trí lưới cuối cùng để debug

    private SoilTile GetTargetSoilTile()
    {
        if (CropManager.Instance.playerTransform == null || SoilManager.Instance.groundTilemap == null)
            return null;

        PlayerController pc = CropManager.Instance.playerTransform.GetComponentInParent<PlayerController>();
        Vector2 dir = pc != null ? pc.FacingDirection : Vector2.down;
        Vector3Int playerCell = SoilManager.Instance.groundTilemap.WorldToCell(CropManager.Instance.playerTransform.position);
        Vector3Int targetCell = playerCell + new Vector3Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), 0);
        Vector2Int gridPos = new Vector2Int(targetCell.x, targetCell.y);

        return SoilManager.Instance.GetTile(gridPos);
    }

    // hàm lấy vị trí ngay trước mặt người chơi 
    private Vector3? GetFrontCellWrldPos()
    {
        if (CropManager.Instance.playerTransform == null || SoilManager.Instance.groundTilemap == null)
            return null;
        PlayerController pc=CropManager.Instance .playerTransform.GetComponentInParent<PlayerController>();
        Vector2 dir=pc!=null?pc.FacingDirection : Vector2.down;

        Vector3Int playerCell = SoilManager.Instance.groundTilemap.WorldToCell(CropManager.Instance.playerTransform.position);
        Vector3Int targetCell = playerCell + new Vector3Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), 0);

        return SoilManager.Instance.groundTilemap.GetCellCenterWorld(targetCell);
    }
    private Vector2Int GetLastGridPos()
    {
        return lastGridPos ?? new Vector2Int(-1, -1); // Trả về (-1, -1) nếu chưa có
    }

    // Gán seed hiện tại từ hotbar/inventory
    public void SetCurrentSeed(CropData seed)
    {
        currentSeed = seed;
    }
}
