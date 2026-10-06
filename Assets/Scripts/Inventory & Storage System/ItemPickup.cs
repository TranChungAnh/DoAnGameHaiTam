using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Thông tin vật phẩm")]
    public ItemBase itemData; // Kéo File ScriptableObject của Item này vào đây
    public int amount = 1;    // Số lượng nhặt (mặc định 1)

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Vector2 clickPos = Mouse.current.position.ReadValue();
            TryPickup(clickPos);
        }

      
    }

    private void TryPickup(Vector2 screenPos)
    {
        // Bỏ qua nếu người chơi đang bấm trên giao diện UI (Hotbar, Bag,...)
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        // Nếu Raycast trúng đúng GameObject này
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        if (itemData == null)
        {
            Debug.LogError($"❌ {gameObject.name} chưa gán itemData (ItemBase)!");
            return;
        }

        // Thêm item vào Inventory thông qua InventoryManager đã có sẵn
        bool wasAdded = InventoryManager.Instance.AddItem(itemData, amount);

        if (wasAdded)
        {
            Debug.Log($"🎒 Đã nhặt {amount}x {itemData.itemName} vào kho!");

            // Cập nhật lại giao diện Inventory / Storage nếu đang mở
            UIInventory uiInv = FindObjectOfType<UIInventory>();
            if (uiInv != null && uiInv.gameObject.activeInHierarchy)
            {
                uiInv.Refresh();
            }

            // Hóa/Xóa vật phẩm dưới đất sau khi đã nhặt
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("⚠ Kho đồ đã đầy, không thể nhặt thêm!");
        }
    }
}