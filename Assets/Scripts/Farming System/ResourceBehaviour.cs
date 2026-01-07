using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBehaviour : MonoBehaviour
{
    [Header("Resource Data")]
    public ResourceData data;
    private int currentHealth;

    public void Start()
    {
        if (data != null)
        {
            currentHealth = data.maxHealth;
        }
    }

    public void TakeDamage(int dmg, ToolType toolUsed)
    {
        if (data == null) return;
        if (!ToolHelper.IsValidTool(data.type, toolUsed)) return;

        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            Harvest();
        }
    }

    private void Harvest()
    {
        DropItems();
        ResourceSpawner.Instance?.OnResourceDestroyed(this.gameObject, data);
        Destroy(gameObject);
    }

    private void DropItems()
    {
        if (data.lootTable == null || data.lootTable.entries.Length == 0) return;

        //  tìm StorageContainer (có thể gắn vào Player hoặc global singleton)
        var storage = FindObjectOfType<StorageContainer>();
        if (storage == null)
        {
            Debug.LogWarning("⚠ Không tìm thấy StorageContainer để thêm item!");
            return;
        }

        foreach (var entry in data.lootTable.entries)
        {
            if (Random.value <= entry.dropChance)
            {
                int count = Random.Range(entry.minCount, entry.maxCount + 1);

                // Thêm thẳng vào kho
                bool added = storage.AddItem(entry.itemRef, count);
                if (added)
                    Debug.Log($"+ {count} {entry.itemRef.itemName} đã thêm vào kho");
                else
                    Debug.Log("⚠ Kho đầy, không thể thêm item!");
            }
        }
    }
}
