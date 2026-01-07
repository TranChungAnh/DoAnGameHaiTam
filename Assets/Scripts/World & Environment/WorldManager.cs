using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    [Header("Bootstrap")]
    public string firstScene = "Main";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Ở đây có thể khởi tạo save/load, hoặc load scene đầu
        if (SceneManager.GetActiveScene().name != firstScene)
            SceneManager.LoadScene(firstScene);
    }

    public void LoadWorld(string worldName)
    {
        SceneManager.LoadScene(worldName);
    }

    // Placeholder save/load
    public void SaveGame()
    {
        PlayerPrefs.SetInt("Day", TimeManager.Instance.Day);
        PlayerPrefs.SetInt("Month", TimeManager.Instance.Month);
        PlayerPrefs.SetInt("Year", TimeManager.Instance.Year);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        // Ví dụ đơn giản: chỉ đọc lại ngày tháng năm
        // (Thực tế cần hệ thống serialize cho crops/animals, v.v.)
        // Ở demo này mình không tự ý ghi đè thời gian đang chạy.
    }
}
