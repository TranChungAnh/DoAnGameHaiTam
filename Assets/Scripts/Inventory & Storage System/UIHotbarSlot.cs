using UnityEngine;
using UnityEngine.EventSystems;

public class UIHotbarSlot : UIItemSlot, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        if (boundSlot != null && !boundSlot.IsEmpty)
        {
            HotbarManager.Instance.OnHotbarItemClicked(boundSlot.item, slotIndex);
        }
        else
        {
            HotbarManager.Instance.SelectSlot(slotIndex);
        }
    }
}
