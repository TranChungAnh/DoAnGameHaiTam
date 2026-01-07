using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform gridRoot;

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in gridRoot) Destroy(child.gameObject);

        var slots = InventoryManager.Instance.playerInventory.slots;
        for (int i = 0; i < slots.Count; i++)
        {
            var go = Instantiate(slotPrefab, gridRoot);
            var uiSlot = go.GetComponent<UIItemSlot>(); // script xử lý drag/drop
            uiSlot.SetData(slots[i], i);
        }
    }
}
