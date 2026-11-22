
using System;
using System.Collections;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
    [Header("Auto Save Settings")]
    [SerializeField] private float saveInterval = 10f;
    [SerializeField] private int slotNumber = 0;
    [SerializeField] private bool autoSaveEnabled = true;

    private void Start()
    {
        StartCoroutine(AutoSaveRoutine());
    }
    private void Awake()
    {
        GameObject existing = GameObject.Find(gameObject.name);

        if (existing != null && existing != gameObject)
        {
            Destroy(existing);
        }

        DontDestroyOnLoad(gameObject);
    }
    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(saveInterval);

            if (!autoSaveEnabled)
                continue;

            if (SaveManager.instance == null)
                continue;

            // Lưu game
            SaveManager.instance.SaveGame(slotNumber);

            // Lưu thời gian
            string description = "Slot " + slotNumber +
                                 " | " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            PlayerPrefs.SetString(
                "Slot" + slotNumber + "Description",
                description
            );

            PlayerPrefs.Save();

            Debug.Log("💾 AUTO SAVE thành công! Slot: " + slotNumber);
        }
    }
}
