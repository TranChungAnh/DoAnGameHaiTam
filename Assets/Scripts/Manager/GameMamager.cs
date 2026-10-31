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

    [SerializeField] public GameDataSO gameData;

    public States states { get; private set; }


    [SerializeField] private GameObject gameOver;
    [SerializeField] private Player player;

    private GameObject newPlayer;
    public bool isGameOver;
    public bool isOpenTeleport = false;

    void Start()
    {
        instance = this;

        CVC = GameObject.Find("PlayerCamera")?.GetComponent<CinemachineVirtualCamera>();
        healthBar = GameObject.Find("HealthBarContainer")?.GetComponent<HealthBar>();

        // 🔹 THÊM ĐOẠN NÀY: Tự động tìm States của Player có sẵn trong Scene khi vừa start
        GameObject existingPlayer = GameObject.FindWithTag("Player");
        if (existingPlayer != null)
        {
            states = existingPlayer.transform.Find("Core/States")?.GetComponent<States>();
            if (states != null && healthBar != null)
            {
                healthBar.States = states;
            }
        }

        isGameOver = false;

        // Xử lý nạp dữ liệu tạm nếu vừa load game từ menu sang
        if (TemporarySaveData.pendingSaveData != null && SaveManager.instance != null)
        {
            SaveManager.instance.ApplyLoadedData(TemporarySaveData.pendingSaveData);
            TemporarySaveData.pendingSaveData = null;
        }
    }

    void Update()
    {

        if (gameData.lives <= 0 && !isGameOver && !playerRespawner.isRespawningPlayer)
        {
            TriggerGameOver();
            if (newPlayer != null)
                newPlayer.SetActive(false);
        }

        CheckRespawn();

       
    }

    public void Respawn()
    {
        if (gameData.lives <= 0) return;

        respawnStartTime = Time.time;
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

    }

 
   
}
