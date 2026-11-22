using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button LoadGameBTN;
    public GameDataSO gameData;

    public void NewGame()
    {
        // Xóa dữ liệu vật phẩm đã nhặt từ các lần chơi trước
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Xóa các file Save Game
        for (int i = 0; i < 3; i++)
        {
            string path = Path.Combine(
                Application.persistentDataPath,
                "SaveGame" + i + ".json"
            );

            if (File.Exists(path))
                File.Delete(path);
        }

        // Xóa dữ liệu Load đang chờ
        TemporarySaveData.pendingSaveData = null;

        // Reset trạng thái game
        gameData.lives = 1;

        // Bắt đầu chơi mới từ Map01
        SceneManager.LoadScene("Map01");
    }

    public void ExitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}