using UnityEngine;
using UnityEngine.UI;

public class UIStorage : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform gridRoot;
    public StorageContainer targetStorage;

    void OnEnable()
    {
        Refresh();
    }

    //public void Refresh()
    //{
    //    foreach (Transform child in gridRoot) Destroy(child.gameObject);

    //    foreach (var slot in targetStorage.slots)
    //    {
    //        var go = Instantiate(slotPrefab, gridRoot);
    //        var icon = go.transform.Find("Icon").GetComponent<Image>();
    //        var txt = go.transform.Find("Text").GetComponent<Text>();

    //        if (!slot.IsEmpty)
    //        {
    //            icon.sprite = slot.item.icon;
    //            icon.enabled = true;
    //            txt.text = slot.quantity.ToString();
    //        }
    //        else
    //        {
    //            icon.enabled = false;
    //            txt.text = "";
    //        }
    //    }
    //}
    public void Refresh()
    {
        foreach (Transform child in gridRoot) Destroy(child.gameObject);

        var slots = targetStorage.slots;
        for (int i = 0; i < slots.Count; i++)
        {
            var go = Instantiate(slotPrefab, gridRoot);
            var uiSlot = go.GetComponent<UIItemSlot>(); // Gắn script drag/drop
            uiSlot.SetData(slots[i], i);
        }
    }
}
