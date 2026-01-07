using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public StorageContainer playerInventory;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool HasItem(string itemName, int amount = 1)
        => playerInventory.HasItem(itemName, amount);

    public bool AddItem(ItemBase item, int amount = 1)
        => playerInventory.AddItem(item, amount);

    public bool RemoveItem(string itemName, int amount = 1)
        => playerInventory.RemoveItem(itemName, amount);
}
