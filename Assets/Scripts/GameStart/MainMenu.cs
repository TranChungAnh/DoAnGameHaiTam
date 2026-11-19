using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button LoadGameBTN;
    public GameDataSO gameData;
    private void Start()
    {

    }
    public void NewGame()
    {
        SceneManager.LoadScene("Map01");
        gameData.lives = 2;
    }
    public void ExitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
