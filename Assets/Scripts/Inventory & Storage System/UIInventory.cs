
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject slotPrefab;
    public Transform gridRoot;

    private List<GameObject> spawnedSlots = new List<GameObject>();

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

        Refresh();
    }

    private void OnDisable()
    {
        // Khôi phục thời gian
        Time.timeScale = previousTimeScale;

        // Khôi phục âm thanh
        AudioListener.pause = previousAudioPause;
    }

    public void Refresh()
    {
        if (gridRoot == null || slotPrefab == null)
            return;

        if (InventoryManager.Instance == null ||
            InventoryManager.Instance.playerInventory == null)
            return;

        foreach (GameObject slot in spawnedSlots)
        {
            if (slot != null)
                Destroy(slot);
        }

        spawnedSlots.Clear();

        var slots = InventoryManager.Instance.playerInventory.slots;

        for (int i = 0; i < slots.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, gridRoot);

            spawnedSlots.Add(go);

            UIItemSlot uiSlot = go.GetComponent<UIItemSlot>();

            if (uiSlot != null)
                uiSlot.SetData(slots[i], i);
        }
    }
}
