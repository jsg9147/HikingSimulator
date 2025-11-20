using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombinationSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon; // 아이템 아이콘
    public bool isResultSlot; // 결과 슬롯 여부
    private ItemData item; // 슬롯에 있는 아이템 데이터
    private InventorySlot originalSlot; // 원래 아이템이 있던 슬롯

    private void Start()
    {
        if (icon == null)
        {
            icon = GetComponent<Image>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && item != null)
        {
            HandleRightClick();
        }
    }

    private void HandleRightClick()
    {
        if (isResultSlot)
        {
            MoveItemToInventory();
        }
        else
        {
            ReturnItemToOriginalSlot();
            CombinationUIManager.instance.AttemptCombination();
        }
    }

    private void MoveItemToInventory()
    {
        InventoryManager.instance.AddItem(item);
        CombinationUIManager.instance.GetCombinationItem();
        ClearSlot();
    }

    private void ReturnItemToOriginalSlot()
    {
        if (originalSlot != null)
        {
            ItemData originalSlotItem = originalSlot.GetItem();
            if (originalSlotItem == null)
            {
                originalSlot.SetItem(item);
            }
            else if (originalSlotItem.isStackable)
            {
                originalSlot.IncreaseQuantity();
                originalSlot.UpdateQuantityText();
            }
        }
        ClearSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryUIManager.instance.DescriptionPopupOn(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUIManager.instance.DescriptionPopupOff();
    }

    public void SetItem(ItemData newItem, InventorySlot originalInventorySlot = null)
    {
        item = newItem;
        originalSlot = originalInventorySlot;
        icon.sprite = item.icon;
        icon.enabled = true;

        if (!isResultSlot)
        {
            CombinationUIManager.instance.AttemptCombination();
        }
    }

    public void ClearSlot()
    {
        item = null;
        originalSlot = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public ItemData GetItem()
    {
        return item;
    }
}