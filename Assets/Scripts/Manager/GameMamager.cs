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
    [SerializeField] private Player player; // Kéo thả Player trực tiếp vào đây trong Inspector

    public bool isGameOver;
    public bool isOpenTeleport = false;

    void Start()
    {
        instance = this;

        // 🔹 THÊM ĐOẠN NÀY ĐỂ RESET SỐ MẠNG KHI BẮT ĐẦU GAME
        if (gameData != null)
        {
            gameData.lives = 1; // Bạn có thể đổi số 3 thành số mạng mặc định bạn muốn
        }

        CVC = GameObject.Find("PlayerCamera")?.GetComponent<CinemachineVirtualCamera>();
        healthBar = GameObject.Find("HealthBarContainer")?.GetComponent<HealthBar>();

        // Tự động tìm States của Player có sẵn trong Scene khi vừa start
        if (player != null)
        {
            states = player.transform.Find("Core/States")?.GetComponent<States>();
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
            if (player != null)
                player.gameObject.SetActive(false);
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
        // Player respawn (Tái sử dụng Player cũ)
        if (!isGameOver && playerRespawner.isRespawningPlayer && Time.time >= respawnStartTime + playerRespawner.respawnTime && gameData.lives > 0)
        {
            playerRespawner.isRespawningPlayer = false;

            if (player != null)
            {
                // Lấy điểm hồi sinh đầu tiên
                Transform respawnPoint = playerRespawner.playerRespawnPoints[0];

                // Tắt tạm CharacterController (nếu có) để tránh bị kẹt vị trí cũ khi đổi tọa độ
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                // Dịch chuyển Player về vị trí và hướng của điểm hồi sinh
                player.transform.position = respawnPoint.position;
                player.transform.rotation = respawnPoint.rotation;

                if (cc != null) cc.enabled = true;

                // Bật lại Player lên
                player.gameObject.SetActive(true);

                // Cập nhật lại Camera follow mục tiêu cũ và ép bám sát ngay lập tức
                if (CVC != null)
                {
                    CVC.m_Follow = player.transform;
                    CVC.m_LookAt = player.transform;
                    CVC.PreviousStateIsValid = false;
                }

                // Hồi đầy máu và kết nối lại thanh máu
                states = player.transform.Find("Core/States")?.GetComponent<States>();
                if (states != null)
                {
                    states.currentHealth = states.maxHealth;
                    if (healthBar != null)
                    {
                        healthBar.States = states;
                    }
                }
            }

            gameData.lives--;
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");

        if (gameOver != null)
            gameOver.SetActive(true);
    }
}