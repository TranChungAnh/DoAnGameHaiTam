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
    public bool isSavingToJson = true;
    public bool isLoading;
    string jsonPathProject;
    string jsonPathPersistent;
    string binarypath;
    public EntityRespawner playerRespawner;

    string fileName = "SaveGame";
    private void Start()
    {
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar;
        jsonPathPersistent = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        binarypath = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
    }


    #region chọn cách lưu fiel 
    #region Save
    public void SaveGame(int slotNumber)
    {
        AllGameData data = new AllGameData();
        data.playerSaves = GetPlayerSaves();
        SavingTypeSwitch(data, slotNumber);
    }
    private PlayerSaves GetPlayerSaves()
    {
        if (gameManager == null || gameManager.states == null)
        {
            Debug.LogError("GameManager hoặc States chưa được gán!");
            return new PlayerSaves(100, Vector3.zero, 0, 3); // fallback default
        }

        float playerStates = gameManager.states.currentHealth;
        Vector3 playerPos = playerRespawner.transform.position;
        int killCount = gameManager.gameData.killEnemy;
        int lives = gameManager.gameData.lives;

        return new PlayerSaves(playerStates, playerPos, killCount, lives);
    }
    public void SavingTypeSwitch(AllGameData gameData, int slotNumber)
    {
        if (isSavingToJson)
        {
            SaveGameDataToJsonFile(gameData, slotNumber);
        }
        else
        {
            SaveGameDataToBinaryFile(gameData, slotNumber);
        }
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
            AllGameData gameData = LoadGameDataFromBinaryFile(slotNumber);
            return gameData;
        }
    }
    public void LoadGame(int slotNumber)
    {
        // Player Data 
        SetPlayerData(LoadingTypeSwitch(slotNumber).playerSaves);


    }
    private void SetPlayerData(PlayerSaves playerSaves)
    {
        gameManager.states.currentHealth = playerSaves.playerStates;
        playerRespawner.transform.position = playerSaves.playerPossition;
        gameManager.gameData.killEnemy = playerSaves.killCount;
        gameManager.gameData.lives = playerSaves.lives;
        Debug.Log("Lives của 1 :" + playerSaves.lives);

    }
    #region To binary section( lưu băng nhị phân)
    public void SaveGameDataToBinaryFile(AllGameData gameData, int slotNumber)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(binarypath + fileName + slotNumber + ".bin", FileMode.Create);
        formatter.Serialize(stream, gameData);
        stream.Close();
        print("Data saved to" + binarypath + fileName + slotNumber + ".bin");
    }
    // tải dữ liệu bằng binary 
    public AllGameData LoadGameDataFromBinaryFile(int slotNumber)
    {
        if (File.Exists(binarypath + fileName + slotNumber + ".bin"))
        {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binarypath + fileName + slotNumber + ".bin", FileMode.Open);
            AllGameData gameData = formatter.Deserialize(stream) as AllGameData;
            stream.Close();
            print("Data loaded from" + binarypath + fileName + slotNumber + ".bin");
            return gameData;
        }
        else
        {
            return null;
        }
    }

    #endregion
    #region To Json section( lưu bằng Json)
    public void SaveGameDataToJsonFile(AllGameData gameData, int slotNumber)
    {
        string json = JsonUtility.ToJson(gameData);
        string encryptedJson = EncryptionDecryption(json);
        using (StreamWriter writer = new StreamWriter(jsonPathProject + fileName + slotNumber + ".json"))
        {
            writer.Write(encryptedJson);
            print("Đã lưu trò chơi vào tệp Json tại:" + jsonPathProject + fileName + slotNumber + ".json");
        };

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
        using (StreamReader reader = new StreamReader(jsonPathProject + fileName + ".json"))
        {
            string json = reader.ReadToEnd();
            string decrypted = EncryptionDecryption(json);
            AllGameData gameData = JsonUtility.FromJson<AllGameData>(decrypted);
            return gameData;
        }
    }
    #endregion

    public void StartLoadedGame(int slotNumber)
    {
        isLoading = true;
        SceneManager.LoadScene("Part2");
        StartCoroutine(DeplayedLoading(slotNumber));
    }

    private IEnumerator DeplayedLoading(int slotNumber)
    {
        yield return new WaitForSeconds(3f);
        LoadGame(slotNumber);
        print("Game Loaded");
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