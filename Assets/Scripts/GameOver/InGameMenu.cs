
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameDataSO gamedata;

    private float previousTimeScale = 1f;
    private bool previousAudioPause;

    private void OnEnable()
    {
        previousTimeScale = Time.timeScale;
        previousAudioPause = AudioListener.pause;

        // Dừng gameplay
        Time.timeScale = 0f;

        // Tắt toàn bộ âm thanh
        AudioListener.pause = true;
    }

    private void OnDisable()
    {
        // Khôi phục thời gian
        Time.timeScale = previousTimeScale;

        // Khôi phục âm thanh
        AudioListener.pause = previousAudioPause;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene("GameStart");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

        if (gamedata != null)
        {
            gamedata.lives = 1;
        }
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }
}
