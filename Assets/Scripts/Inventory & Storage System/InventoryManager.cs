
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public StorageContainer playerInventory;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Nếu chưa kéo thả trên Inspector, code sẽ tự quét tìm StorageContainer trong scene
            if (playerInventory == null)
            {
                playerInventory = FindObjectOfType<StorageContainer>();
            }

            // Đưa InventoryManager ra Root và giữ lại qua các scene
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            // QUAN TRỌNG: Giữ luôn cả GameObject chứa StorageContainer đi theo cùng
            if (playerInventory != null && playerInventory.gameObject != gameObject)
            {
                playerInventory.transform.SetParent(null);
                DontDestroyOnLoad(playerInventory.gameObject);
            }

            Debug.Log("✅ Đã giữ lại hệ thống Inventory qua scene thành công!");
        }
        else
        {
            // Nếu sang scene mới mà có object thừa, tự động hủy bản sao thừa đi
            if (playerInventory != null && playerInventory != Instance.playerInventory)
            {
                Destroy(playerInventory.gameObject);
            }
            Destroy(gameObject);
        }
    }
    public bool HasItem(string itemName, int amount = 1)
    {
        return playerInventory != null &&
               playerInventory.HasItem(itemName, amount);
    }

    public bool AddItem(ItemBase item, int amount = 1)
    {
        return playerInventory != null &&
               playerInventory.AddItem(item, amount);
    }

    public bool RemoveItem(string itemName, int amount = 1)
    {
        return playerInventory != null &&
               playerInventory.RemoveItem(itemName, amount);
    }

    public int GetItemCount(ItemBase item)
    {
        if (playerInventory == null) return 0;

        return playerInventory.GetItemCount(item);
    }
}
