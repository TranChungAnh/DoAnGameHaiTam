
using System;
using System.Collections;
using UnityEngine;

public class States : CoreComponent
{
    public static States Instance { get; set; }
    public event Action OnHealthZero;

    public float maxHealth;
    public float currentHealth;

    [Header("Health Regeneration Settings")]
    [SerializeField] private bool enableRegen = true;

    [Header("Player Regen Stats")]
    [SerializeField] private float playerRegenDelay = 3f;
    [SerializeField] private float playerRegenRate = 5f;

    [Header("Enemy Regen Stats")]
    [SerializeField] private float enemyRegenDelay = 6f;
    [SerializeField] private float enemyRegenRate = 3f;

    private GameMamager GM;
    private Coroutine regenCoroutine;
    private float lastDamageTime;
    private bool isPlayer;

    private Transform playerRoot;
    private Transform enemyRoot;

    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        GM = GameObject.Find("GameMamager")?.GetComponent<GameMamager>();

        // Tìm đúng Player hoặc Enemy chứa States
        playerRoot = FindTaggedAncestor("Player");
        enemyRoot = FindTaggedAncestor("Enemy");

        isPlayer = playerRoot != null;
    }

    private Transform FindTaggedAncestor(string tag)
    {
        Transform current = transform;

        while (current != null)
        {
            if (current.CompareTag(tag))
            {
                return current;
            }

            current = current.parent;
        }

        return null;
    }

    public void DecreaseHealth(float amount)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= amount;
        lastDamageTime = Time.time;

        // Dừng hồi máu nếu tiếp tục nhận sát thương
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnHealthZero?.Invoke();
            Die();
            return;
        }

        if (enableRegen)
        {
            regenCoroutine = StartCoroutine(RegenHealthRoutine());
        }
    }

    public void IncreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public void setHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
    }

    private IEnumerator RegenHealthRoutine()
    {
        float actualDelay = isPlayer ? playerRegenDelay : enemyRegenDelay;
        float actualRate = isPlayer ? playerRegenRate : enemyRegenRate;

        // Chờ hết thời gian không nhận sát thương
        while (Time.time < lastDamageTime + actualDelay)
        {
            yield return null;
        }

        // Hồi máu liên tục
        while (currentHealth < maxHealth && currentHealth > 0)
        {
            currentHealth += actualRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            yield return null;
        }

        regenCoroutine = null;
    }

    public void Die()
    {
        // Dừng hồi máu khi chết
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        if (isPlayer)
        {
            if (GM != null && playerRoot != null)
            {
                GM.playerRespawner.isRespawningPlayer = true;
                GM.Respawn();

                // Ẩn Player
                playerRoot.gameObject.SetActive(false);
            }
        }
        else
        {
            if (enemyRoot == null)
            {
                Debug.LogWarning("Không tìm thấy Enemy chứa States: " + gameObject.name);
                return;
            }

            string enemyBaseName = enemyRoot.name.Replace("(Clone)", "").Trim();

            Transform tempRespawnPoint = new GameObject(
                "TempRespawnPoint_" + enemyBaseName
            ).transform;

            tempRespawnPoint.position = enemyRoot.position;

            if (GM != null)
            {
                var enemyData = new EntityRespawner.EnemyRespawnData
                {
                    enemyName = enemyBaseName,
                    prefab = GM.enemyPrefabs.Find(p => p.name == enemyBaseName),
                    respawnPoint = tempRespawnPoint
                };

                GM.playerRespawner.enemiesToRespawn.Enqueue(enemyData);

                GM.Respawn();
            }

            // Chỉ hủy đúng Enemy đang chết
            Destroy(enemyRoot.gameObject);
        }
    }
}
