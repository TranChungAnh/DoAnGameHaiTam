using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
                          IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public Text quantityText;
    public UIItemInfo itemInfoPanel;
    [HideInInspector] public StorageSlot boundSlot;
    [HideInInspector] public int slotIndex;

    private Transform originalParent;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (itemInfoPanel == null)
        {
            itemInfoPanel = FindObjectOfType<UIItemInfo>(true);
            if (itemInfoPanel == null)
            {
                Debug.LogError("❌ Không tìm thấy UIItemInfo trong scene!");
            }
        }
    }

    public void SetData(StorageSlot slot, int index)
    {
        boundSlot = slot;
        slotIndex = index;
        Refresh();
    }

    public virtual void Refresh()
    {
        if (boundSlot != null && !boundSlot.IsEmpty)
        {
            icon.sprite = boundSlot.item.icon;
            icon.enabled = true;
            quantityText.text = boundSlot.quantity.ToString();
        }
        else
        {
            icon.enabled = false;
            quantityText.text = "";
        }
    }

    #region Tooltip Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (boundSlot != null && !boundSlot.IsEmpty)
        {
            // Hiện tooltip ngay trên đầu slot
            UITooltip.Show(boundSlot.item.itemName, transform.position + new Vector3(0, 50, 0));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITooltip.Hide();
    }
    #endregion

    #region Drag & Drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (boundSlot == null || boundSlot.IsEmpty) return;

        originalParent = icon.transform.parent;
        icon.transform.SetParent(canvas.transform);
        icon.raycastTarget = false;
        icon.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (boundSlot == null || boundSlot.IsEmpty) return;
        icon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        icon.transform.SetParent(originalParent);
        icon.transform.localPosition = Vector3.zero;
        icon.raycastTarget = true;
        Refresh();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<UIItemSlot>();
        if (dragged == null || dragged == this) return;

        var tempItem = boundSlot.item;
        var tempQty = boundSlot.quantity;

        boundSlot.item = dragged.boundSlot.item;
        boundSlot.quantity = dragged.boundSlot.quantity;

        dragged.boundSlot.item = tempItem;
        dragged.boundSlot.quantity = tempQty;

        Refresh();
        dragged.Refresh();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (boundSlot != null && !boundSlot.IsEmpty)
        {
            itemInfoPanel.ShowInfo(boundSlot.item);
        }
    }
    #endregion
}
