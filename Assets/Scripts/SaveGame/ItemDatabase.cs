using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("Kéo tất cả các file ItemBase của game vào đây")]
    public List<ItemBase> allItems = new List<ItemBase>();

    private Dictionary<string, ItemBase> itemLookup = new Dictionary<string, ItemBase>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BuildDatabase()
    {
        itemLookup.Clear();
        foreach (var item in allItems)
        {
            if (item != null && !string.IsNullOrEmpty(item.itemID))
            {
                if (!itemLookup.ContainsKey(item.itemID))
                {
                    itemLookup.Add(item.itemID, item);
                }
            }
        }
    }

    // Hàm lấy lại ItemBase dựa vào ID đã lưu
    public ItemBase GetItemByID(string id)
    {
        if (itemLookup.TryGetValue(id, out ItemBase item))
        {
            return item;
        }
        Debug.LogWarning($"⚠ Không tìm thấy item với ID: {id}");
        return null;
    }
}