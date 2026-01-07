
using UnityEngine;
using UnityEngine.UI;

public class UIItemInfo : MonoBehaviour
{
    public Image icon;
    public Text itemName;
    public Text description;

    private void Start()  
    {
        gameObject.SetActive(false);
    }

    public void ShowInfo(ItemBase item)
    {
        if (item == null) return;

        icon.sprite = item.icon;
        itemName.text = item.itemName;
        description.text = item.description;
        gameObject.SetActive(true);
    }
    public void HideInfo()
    {
        gameObject.SetActive(false);
    }
}
