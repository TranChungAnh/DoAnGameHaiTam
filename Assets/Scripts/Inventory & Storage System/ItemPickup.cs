
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Thông tin vật phẩm")]
    public ItemBase itemData;
    public int amount = 1;

    [Header("Trạng thái thế giới")]
    [Tooltip("Bật: Ghi nhớ vật phẩm đã nhặt qua các lần Load Game.")]
    [SerializeField] private bool rememberAcrossLoads = true;

    [Tooltip("ID riêng của vật phẩm. Đổi ID để vật phẩm xuất hiện lại.")]
    public string uniqueItemID;

    private Camera mainCam;

    private string SaveKey
    {
        get
        {
            return "ItemState_" + uniqueItemID;
        }
    }

    private void OnValidate()
    {
        // Chỉ tạo ID khi chưa có.
        // Không tự thay đổi ID đã được tạo.
        if (string.IsNullOrEmpty(uniqueItemID))
        {
            uniqueItemID = gameObject.name + "_" +
                           System.Guid.NewGuid().ToString("N");

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        CheckCollectedItem();
    }

    private void CheckCollectedItem()
    {
        if (!rememberAcrossLoads)
            return;

        if (string.IsNullOrEmpty(uniqueItemID))
            return;

        // Nếu ID đã được lưu là nhặt rồi
        // thì ẩn vật phẩm ngay khi vào Scene.
        if (PlayerPrefs.GetInt(SaveKey, 0) == 1)
        {
            gameObject.SetActive(false);

            Debug.Log("📦 Vật phẩm đã được nhặt trước đó, ẩn khỏi map: "
                      + uniqueItemID);
        }
    }

    private void Update()
    {
        if (Mouse.current != null &&
            Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Vector2 clickPos = Mouse.current.position.ReadValue();

            TryPickup(clickPos);
        }
    }

    private void TryPickup(Vector2 screenPos)
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current
            .IsPointerOverGameObject())
        {
            return;
        }

        if (mainCam == null)
            mainCam = Camera.main;

        if (mainCam == null)
            return;

        Ray ray = mainCam.ScreenPointToRay(screenPos);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null &&
            hit.collider.gameObject == gameObject)
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
            // Giữ nguyên xử lý của bạn.
        }

        Debug.Log("⚔️ Đã kích hoạt chiến đấu và khóa các hành động khác!");
    }

    private void Pickup()
    {
        if (itemData == null)
        {
            Debug.LogError(
                $"❌ {gameObject.name} chưa gán itemData!"
            );

            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("❌ InventoryManager.Instance bị NULL!");

            return;
        }

        bool wasAdded = InventoryManager.Instance.AddItem(
            itemData,
            amount
        );

        if (wasAdded)
        {
            Debug.Log(
                $"🎒 Đã nhặt {amount}x {itemData.itemName} vào kho!"
            );

            if (SoundManager.Instance != null &&
                SoundManager.Instance.nhatCoVat != null)
            {
                SoundManager.Instance.PlaySound(
                    SoundManager.Instance.nhatCoVat
                );
            }

            UIInventory uiInv = FindObjectOfType<UIInventory>();

            if (uiInv != null &&
                uiInv.gameObject.activeInHierarchy)
            {
                uiInv.Refresh();
            }

            // Lưu trạng thái vật phẩm đã nhặt.
            if (rememberAcrossLoads &&
                !string.IsNullOrEmpty(uniqueItemID))
            {
                PlayerPrefs.SetInt(SaveKey, 1);

                PlayerPrefs.Save();

                Debug.Log(
                    "💾 Đã lưu trạng thái vật phẩm: "
                    + uniqueItemID
                );
            }

            // Ẩn vật phẩm khỏi map.
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "⚠ Kho đồ đã đầy, không thể nhặt thêm!"
            );
        }
    }
}
