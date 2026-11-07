using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameDataSO gamedata;
    public void BackToMenu()
    {
        SceneManager.LoadScene("GameStart");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Part1");
        gamedata.lives = 3;
    }

    public void Back()
    {
        gameObject.SetActive(false);
       
    }
}
