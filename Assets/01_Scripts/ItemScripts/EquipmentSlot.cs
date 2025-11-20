using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData.EquipmentType equipmentType; // 장비 타입
    public Image icon; // 아이템 아이콘
    private ItemData item; // 슬롯에 있는 아이템 데이터

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                if (item.equipmentType == ItemData.EquipmentType.Backpack)
                {
                    Debug.Log("Cannot unequip backpack.");
                }
                else
                {
                    InventoryManager.instance.UnequipItem(item);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryUIManager.instance.DescriptionPopupOn(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUIManager.instance.DescriptionPopupOff();
    }

    public void SetItem(ItemData newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public ItemData GetItem()
    {
        return item;
    }
}
