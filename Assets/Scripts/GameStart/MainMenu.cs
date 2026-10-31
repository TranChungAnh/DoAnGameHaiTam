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
        SceneManager.LoadScene("SampleScene");
        gameData.lives = 3;
    }
    public void ExitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
