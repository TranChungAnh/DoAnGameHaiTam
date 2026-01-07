using System.Collections.Generic;
using UnityEngine;

public class StorageContainer : MonoBehaviour
{
    [Header("Số ô tối đa trong kho")]
    public int capacity = 44;
    public List<StorageSlot> slots = new();

    void Awake()
    {
        // Đảm bảo luôn đủ số slot bằng capacity
        if (slots.Count < capacity)
        {
            for (int i = slots.Count; i < capacity; i++)
                slots.Add(new StorageSlot());
        }
        else if (slots.Count > capacity)
        {
            slots.RemoveRange(capacity, slots.Count - capacity);
        }
    }

    public bool HasItem(string itemName, int amount = 1)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item.itemName == itemName && slot.quantity >= amount)
                return true;
        }
        return false;
    }

    public bool RemoveItem(string itemName, int amount = 1)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item.itemName == itemName && slot.quantity >= amount)
            {
                slot.quantity -= amount;
                if (slot.quantity <= 0) slot.Clear();
                return true;
            }
        }
        return false;
    }

    public bool AddItem(ItemBase newItem, int amount = 1)
    {
        // Nếu item đã có sẵn thì cộng thêm
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item == newItem)
            {
                slot.quantity += amount;
                return true;
            }
        }

        // Nếu chưa có thì tìm slot trống
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.item = newItem;
                slot.quantity = amount;
                return true;
            }
        }
        Debug.Log("⚠ Kho đã đầy!");
        return false;
    }
}
