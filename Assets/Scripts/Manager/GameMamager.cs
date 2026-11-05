using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static PlayerInputHandler;

public class GameMamager : MonoBehaviour
{
    public static GameMamager instance { get; private set; }
    public List<GameObject> enemyPrefabs;

    public EntityRespawner playerRespawner;
    private float respawnStartTime;

    private CinemachineVirtualCamera CVC;
    private HealthBar healthBar;
    public TextMeshProUGUI livesCounter;

    [SerializeField] public GameDataSO gameData;

    public States states { get; private set; }

    public TextMeshProUGUI killCounter;

    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject results;
    [SerializeField] private Player player;

    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI monstersDefeatedText;
    private TextMeshProUGUI survivalTimeText;
    private GameObject newPlayer;
    public bool isGameOver;
    public int enemyCount;
    public bool isOpenTeleport = false;

    void Start()
    {
        instance = this;

        CVC = GameObject.Find("PlayerCamera")?.GetComponent<CinemachineVirtualCamera>();
        healthBar = GameObject.Find("HealthBarContainer")?.GetComponent<HealthBar>();

        scoreText = results.transform.Find("Score")?.GetComponent<TextMeshProUGUI>();
        monstersDefeatedText = results.transform.Find("MonstersDefeated")?.GetComponent<TextMeshProUGUI>();
        survivalTimeText = results.transform.Find("SurvivalTime")?.GetComponent<TextMeshProUGUI>();
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log("Số lượng enemy: " + enemyCount);

        isGameOver = false;
    }

    void Update()
    {
        livesCounter.text = "X" + gameData.lives.ToString();
        killCounter.text = gameData.killEnemy.ToString();

        if (gameData.lives <= 0 && !isGameOver && !playerRespawner.isRespawningPlayer)
        {
            TriggerGameOver();
            if (newPlayer != null)
                newPlayer.SetActive(false);
        }

        CheckRespawn();

        if (enemyCount <= 0)
        {
            isOpenTeleport = true;
        }
    }

    public void Respawn()
    {
        if (gameData.lives <= 0) return;

        respawnStartTime = Time.time;
        Debug.Log("Player will respawn...");
    }

    private void CheckRespawn()
    {
        // Player respawn
        if (!isGameOver && playerRespawner.isRespawningPlayer && Time.time >= respawnStartTime + playerRespawner.respawnTime && gameData.lives > 0)
        {
            playerRespawner.isRespawningPlayer = false;

            newPlayer = Instantiate(playerRespawner.playerPrefabs[0], playerRespawner.playerRespawnPoints[0].position, playerRespawner.playerRespawnPoints[0].rotation);
            newPlayer.name = playerRespawner.playerPrefabs[0].name;

            if (CVC != null)
                CVC.m_Follow = newPlayer.transform;

            states = newPlayer.transform.Find("Core/States")?.GetComponent<States>();
            if (states != null)
            {
                states.currentHealth = states.maxHealth;
                healthBar.States = states;
            }

            gameData.lives--;
        }

        // Enemy respawn
        //else if (!isGameOver && playerRespawner.enemiesToRespawn.Count > 0 && Time.time >= respawnStartTime + playerRespawner.respawnTime)
        //{
        //    var enemyData = playerRespawner.enemiesToRespawn.Dequeue();

        //    if (enemyData.prefab != null && enemyData.respawnPoint != null)
        //    {
        //        GameObject newEnemy = Instantiate(enemyData.prefab, enemyData.respawnPoint.position, Quaternion.identity);
        //        newEnemy.name = enemyData.prefab.name;

        //        states = newEnemy.transform.Find("Core/States")?.GetComponent<States>();
        //        if (states != null)
        //        {
        //            states.currentHealth = states.maxHealth;
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Enemy data invalid for respawn.");
        //    }
        //}
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");

        gameOver.SetActive(true);
        scoreText.text = "Score: " + (gameData.killEnemy * 10).ToString();
        monstersDefeatedText.text = "Monsters Defeated: " + gameData.killEnemy.ToString();

        float time = Time.time;
        float survivalTime = time - player.gameStartTime;
        survivalTimeText.text = "Survival Time: " + FormatTime(survivalTime);
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void EnemyKilled()
    {
        gameData.killEnemy += 1;
        enemyCount -= 3;
    }
}
