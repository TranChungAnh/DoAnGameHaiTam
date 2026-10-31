using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public string itemID;   // Lưu bằng ID thay vì tên
    public int quantity;

    public InventorySaveData(string id, int qty)
    {
        itemID = id;
        quantity = qty;
    }
}

[System.Serializable]
public class PlayerSaves
{
    public string currentScene;             // Tên map hiện tại
    public Vector3 playerPosition;          // Vị trí (x, y, z) của player
    public float currentHealth;             // Máu hiện tại
    public int lives;                       // Mạng còn lại
    public List<InventorySaveData> inventoryItems; // Danh sách đồ trong kho

    public PlayerSaves(string scene, Vector3 pos, float health, int liveCount, List<InventorySaveData> items)
    {
        currentScene = scene;
        playerPosition = pos;
        currentHealth = health;
        lives = liveCount;
        inventoryItems = items;
    }
}