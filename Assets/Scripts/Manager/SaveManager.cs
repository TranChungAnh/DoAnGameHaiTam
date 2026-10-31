using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance { get; set; }
    public GameMamager gameManager;
    [Header("Tham chiếu trực tiếp")]
    public States playerStates; 
    public bool isSavingToJson = true;
    public bool isLoading;
    string jsonPathProject;
    string jsonPathPersistent;
    string binarypath;
    public EntityRespawner playerRespawner;

    string fileName = "SaveGame";
    private void Start()
    {
        jsonPathProject = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        jsonPathPersistent = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        binarypath = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
    }


    #region chọn cách lưu fiel 
    #region Save
    public void SaveGame(int slotNumber)
    {
        AllGameData data = new AllGameData();
        data.playerSaves = GetPlayerSaves();

        // 🔹 THÊM ĐOẠN NÀY ĐỂ IN LOG RA CONSOLE KHI LƯU
        Debug.Log("========== 💾 ĐANG THỰC HIỆN LƯU GAME (Slot " + slotNumber + ") ==========");
        Debug.Log("📍 Map hiện tại (Scene): " + data.playerSaves.currentScene);
        Debug.Log("🎮 Vị trí Player: " + data.playerSaves.playerPosition);
        Debug.Log("❤️ Máu hiện tại: " + data.playerSaves.currentHealth);
        Debug.Log("💖 Số mạng (Lives): " + data.playerSaves.lives);

        if (data.playerSaves.inventoryItems != null && data.playerSaves.inventoryItems.Count > 0)
        {
            Debug.Log("🎒 Danh sách Item trong kho đồ (" + data.playerSaves.inventoryItems.Count + " loại):");
            foreach (var item in data.playerSaves.inventoryItems)
            {
                Debug.Log($"   - Item ID: {item.itemID} | Số lượng: {item.quantity}");
            }
        }
        else
        {
            Debug.Log("🎒 Kho đồ trống!");
        }
        Debug.Log("==========================================================");

        SavingTypeSwitch(data, slotNumber);
    }
    private PlayerSaves GetPlayerSaves()
    {
        // 1. Lấy tên Scene hiện tại
        string currentScene = SceneManager.GetActiveScene().name;

        // 2. Lấy vị trí Player
        Vector3 playerPos = Vector3.zero;
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerPos = playerObj.transform.position; // Lấy trực tiếp tọa độ Player lúc đó
        }
        else if (playerRespawner != null)
        {
            playerPos = playerRespawner.transform.position;
        }
        // 3. 🔹 Tự động tìm trực tiếp máu từ States của Player trên Scene
        float currentHealth = 0f;
        if (playerStates != null)
        {
            currentHealth = playerStates.currentHealth;
            Debug.Log("❤️ Đã lấy máu thành công để lưu: " + currentHealth);
        }
        else
        {
            Debug.LogWarning("⚠ Chưa gán States vào SaveManager trên Inspector!");
        }
        // 4. Lấy số mạng từ GameManager (nếu có)
        int lives = 3;
        if (gameManager == null) gameManager = FindObjectOfType<GameMamager>();
        if (gameManager != null && gameManager.gameData != null)
        {
            lives = gameManager.gameData.lives;
        }

        // 5. Lấy toàn bộ item trong Inventory
        List<InventorySaveData> inventoryList = new List<InventorySaveData>();
        if (InventoryManager.Instance != null && InventoryManager.Instance.playerInventory != null)
        {
            foreach (var slot in InventoryManager.Instance.playerInventory.slots)
            {
                if (slot != null && !slot.IsEmpty && slot.item != null)
                {
                    inventoryList.Add(new InventorySaveData(slot.item.itemID, slot.quantity));
                }
            }
        }

        return new PlayerSaves(currentScene, playerPos, currentHealth, lives, inventoryList);
    }
    public void SavingTypeSwitch(AllGameData gameData, int slotNumber)
    {
        if (isSavingToJson)
        {
            SaveGameDataToJsonFile(gameData, slotNumber);
        }
        //else
        //{
        //    SaveGameDataToBinaryFile(gameData, slotNumber);
        //}
    }

    #endregion
    #region Load
    public AllGameData LoadingTypeSwitch(int slotNumber)
    {
        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameDataFromJsonFile(slotNumber);
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameDataFromJsonFile(slotNumber);
            return gameData;
        }
    }
    public void LoadGame(int slotNumber)
    {
        // Player Data 
        ApplyLoadedData(LoadingTypeSwitch(slotNumber).playerSaves);



    }

    public void ApplyLoadedData(PlayerSaves playerSaves)
    {
        if (playerSaves == null) return;

        // 1. Khôi phục vị trí Player
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            // Tắt CharacterController tạm thời nếu có để tránh bịUnity chặn dịch chuyển vị trí
            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            playerObj.transform.position = playerSaves.playerPosition;

            if (cc != null) cc.enabled = true;

            Debug.Log("📍 Đã khôi phục thành công vị trí Player: " + playerSaves.playerPosition);
        }
        else
        {
            Debug.LogWarning("⚠ Không tìm thấy GameObject có Tag 'Player' để gán vị trí!");
        }
        // 2. 🔹 BỔ SUNG: Khôi phục máu trực tiếp thông qua playerStates đã kéo thả
        if (playerStates != null)
        {
            playerStates.currentHealth = playerSaves.currentHealth;
            Debug.Log("❤️ Đã khôi phục máu thành công: " + playerSaves.currentHealth);
        }
        else
        {
            // Dự phòng nếu playerStates bị mất kết nối khi đổi Scene, tự tìm trên Player
            GameObject pObj = GameObject.FindWithTag("Player");
            if (pObj != null)
            {
                States foundStates = pObj.GetComponentInChildren<States>();
                if (foundStates != null)
                {
                    foundStates.currentHealth = playerSaves.currentHealth;
                    Debug.Log("❤️ Đã tìm và khôi phục máu qua Tag Player: " + playerSaves.currentHealth);
                }
            }
        }
        // 3. Khôi phục số mạng
        if (gameManager == null) gameManager = FindObjectOfType<GameMamager>();
        if (gameManager != null && gameManager.gameData != null)
        {
            gameManager.gameData.lives = playerSaves.lives;
        }

        // 4. Khôi phục lại Inventory
        if (InventoryManager.Instance != null && InventoryManager.Instance.playerInventory != null)
        {
            foreach (var slot in InventoryManager.Instance.playerInventory.slots)
            {
                slot.Clear();
            }

            foreach (var savedItem in playerSaves.inventoryItems)
            {
                if (ItemDatabase.Instance != null)
                {
                    ItemBase loadedItem = ItemDatabase.Instance.GetItemByID(savedItem.itemID);
                    if (loadedItem != null)
                    {
                        InventoryManager.Instance.AddItem(loadedItem, savedItem.quantity);
                    }
                }
            }
        }

        Debug.Log("✅ Đã hoàn tất quá trình tải và khôi phục dữ liệu game!");
    }
    #region To binary section( lưu băng nhị phân)
    //public void SaveGameDataToBinaryFile(AllGameData gameData, int slotNumber)
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    FileStream stream = new FileStream(binarypath + fileName + slotNumber + ".bin", FileMode.Create);
    //    formatter.Serialize(stream, gameData);
    //    stream.Close();
    //    print("Data saved to" + binarypath + fileName + slotNumber + ".bin");
    //}
    // tải dữ liệu bằng binary 
    //public AllGameData LoadGameDataFromBinaryFile(int slotNumber)
    //{
    //    if (File.Exists(binarypath + fileName + slotNumber + ".bin"))
    //    {

    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(binarypath + fileName + slotNumber + ".bin", FileMode.Open);
    //        AllGameData gameData = formatter.Deserialize(stream) as AllGameData;
    //        stream.Close();
    //        print("Data loaded from" + binarypath + fileName + slotNumber + ".bin");
    //        return gameData;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    #endregion
    #region To Json section( lưu bằng Json)
    public void SaveGameDataToJsonFile(AllGameData gameData, int slotNumber)
    {
        string json = JsonUtility.ToJson(gameData);
        string encryptedJson = EncryptionDecryption(json);

        // Dùng persistentDataPath thay vì dataPath để ghi file an toàn
        string fullPath = jsonPathPersistent + fileName + slotNumber + ".json";
        using (StreamWriter writer = new StreamWriter(fullPath))
        {
            writer.Write(encryptedJson);
            print("Đã lưu trò chơi vào tệp Json tại: " + fullPath);
        }
    }
    #endregion
    #endregion
    #region ||--------- Mã hóa file Json  ----------------||
    public string EncryptionDecryption(string jsonString)
    {
        string keyword = "12346710";
        string result = "";
        for (int i = 0; i < jsonString.Length; i++)
        {
            result += (char)(jsonString[i] ^ keyword[i % keyword.Length]);
        }
        return result;
    }
    // tải dữ liệu Json
    public AllGameData LoadGameDataFromJsonFile(int slotNumber)
    {
        string fullPath = jsonPathPersistent + fileName + slotNumber + ".json";
        if (File.Exists(fullPath))
        {
            using (StreamReader reader = new StreamReader(fullPath))
            {
                string json = reader.ReadToEnd();
                string decrypted = EncryptionDecryption(json);
                AllGameData gameData = JsonUtility.FromJson<AllGameData>(decrypted);
                return gameData;
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy file save tại: " + fullPath);
            return null;
        }
    }
    #endregion

    public void StartLoadedGame(int slotNumber)
    {
        isLoading = true;
        AllGameData loadedData = LoadingTypeSwitch(slotNumber);

        if (loadedData != null && loadedData.playerSaves != null)
        {
            // Lưu tạm dữ liệu để Scene mới load lên sẽ tự bắt lấy
            TemporarySaveData.pendingSaveData = loadedData.playerSaves;

            // 🔹 THÊM: Load đúng tên Map đã được lưu thay vì chữ "Part2" cứng
            SceneManager.LoadScene(loadedData.playerSaves.currentScene);
        }
        else
        {
            Debug.LogError("Không thể tải file save hoặc file trống!");
        }
    }


    #endregion
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    public bool DoseFileExitsts(int slotNumber)
    {
        if (isSavingToJson)
        {
            // kiểm tra file có tồn tại ko : vd saveGame0.json 
            if (System.IO.File.Exists(jsonPathProject + fileName + slotNumber + ".json"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (System.IO.File.Exists(binarypath + fileName + slotNumber + ".bin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    public bool isSlotEmpty(int slotNumber)
    {
        // nếu có link file ở vt slotnumber 
        if (DoseFileExitsts(slotNumber))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    // bỏ chọn button 
    public void DeselectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        // bỏ chọn đói tượng đang được chọn

        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }

}