using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Thông tin vật phẩm")]
    public ItemBase itemData;
    public int amount = 1;

    [Header("Trạng thái thế giới (Chống xuất hiện lại khi Load game)")]
    public string uniqueItemID; // Nhập mã ID độc nhất cho từng món đồ trên map

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        /// Kiểm tra vật phẩm đã nhặt trước đó và xóa khi load lại game
        /*
        if (!string.IsNullOrEmpty(uniqueItemID) &&
            PlayerPrefs.GetInt("Collected_" + uniqueItemID, 0) == 1)
        {
            Destroy(gameObject);
        }
        */
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
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            TriggerCombatAndLockActions();
            Pickup();
        }
    }

    private void TriggerCombatAndLockActions()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {

        }

        Debug.Log("⚔️ Đã kích hoạt chiến đấu và khóa các hành động khác!");
    }

    private void Pickup()
    {
        if (itemData == null)
        {
            Debug.LogError($"❌ {gameObject.name} chưa gán itemData (ItemBase)!");
            return;
        }

        bool wasAdded = InventoryManager.Instance.AddItem(itemData, amount);

        if (wasAdded)
        {
            Debug.Log($"🎒 Đã nhặt {amount}x {itemData.itemName} vào kho!");
            if (SoundManager.Instance != null && SoundManager.Instance.nhatCoVat != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.nhatCoVat);
            }
            UIInventory uiInv = FindObjectOfType<UIInventory>();
            if (uiInv != null && uiInv.gameObject.activeInHierarchy)
            {
                uiInv.Refresh();
            }

            // Lưu trạng thái vĩnh viễn là vật phẩm này đã bị nhặt
            /*
            if (!string.IsNullOrEmpty(uniqueItemID))
            {
                PlayerPrefs.SetInt("Collected_" + uniqueItemID, 1);
                PlayerPrefs.Save();
            }
            */
            // Xóa vật phẩm khỏi mặt đất
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("⚠ Kho đồ đã đầy, không thể nhặt thêm!");
        }
    }
}