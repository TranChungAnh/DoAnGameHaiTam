using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class States : CoreComponent
{
    public static States Instance { get; set; }
    public event Action OnHealthZero;
    public float maxHealth;
    public  float currentHealth;


    private GameMamager GM;
    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;
       

    }
    private void Start()
    {
        GM = GameObject.Find("GameMamager").GetComponent<GameMamager>();
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnHealthZero?.Invoke();
            Die();

        }      
    }

    public void IncreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }
    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;

    }
    public void Die()
    {
        if (gameObject.CompareTag("Player"))
        {
            GM.playerRespawner.isRespawningPlayer = true;
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            var rootObject = transform.root.gameObject;
            string enemyBaseName = rootObject.name.Replace("(Clone)", "").Trim();

            Transform tempRespawnPoint = new GameObject("TempRespawnPoint_" + enemyBaseName).transform;
            tempRespawnPoint.position = rootObject.transform.position;

            var enemyData = new EntityRespawner.EnemyRespawnData
            {
                enemyName = enemyBaseName,
                prefab = GM.enemyPrefabs.Find(p => p.name == enemyBaseName),
                respawnPoint = tempRespawnPoint
            };
            GM.playerRespawner.enemiesToRespawn.Enqueue(enemyData);
           
        }
        GM.Respawn();
        Destroy(gameObject);
    }

}
