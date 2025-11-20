using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image frame;
    public Image icon; // 아이템 아이콘
    public TMP_Text quantityText; // 아이템 수량 텍스트
    public bool isCombinationSlot; // 조합 슬롯 여부

    private ItemData item; // 슬롯에 있는 아이템 데이터
    private int quantity; // 아이템 수량

    private Canvas canvas;
    private RectTransform iconRectTransform;
    void Start()
    {
        UpdateQuantityText();
        iconRectTransform = icon.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    // 아이템이 슬롯에 오른쪽 클릭되었을 때 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            HandleRightClick();
        }
    }

    // 마우스가 슬롯에 들어갔을 때 처리
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryUIManager.instance.DescriptionPopupOn(item);
    }

    // 마우스가 슬롯을 떠났을 때 처리
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUIManager.instance.DescriptionPopupOff();
    }

    // 아이템과 수량을 설정
    public void SetItem(ItemData newItem, int newQuantity = 1)
    {
        item = newItem;
        quantity = newQuantity;
        icon.sprite = item.icon;
        icon.enabled = true;
        UpdateQuantityText();

        InventoryUIManager.instance.InventoryUI.SaveItemIndex();
    }

    // 슬롯 비우기
    public void ClearSlot()
    {
        item = null;
        quantity = 0;
        icon.sprite = null;
        icon.enabled = false;
        quantityText.enabled = false;
    }

    // 아이템 가져오기
    public ItemData GetItem()
    {
        return item;
    }

    // 수량 증가
    public void IncreaseQuantity()
    {
        quantity++;
        UpdateQuantityText();
    }

    // 수량 감소
    public void DecreaseQuantity()
    {
        quantity--;
        UpdateQuantityText();
    }

    // 수량 텍스트 업데이트
    public void UpdateQuantityText()
    {
        if (quantity > 1)
        {
            quantityText.text = quantity.ToString();
            quantityText.enabled = true;
        }
        else if (quantity <= 1)
        {
            quantityText.enabled = false;
        }
    }

    // 아이템 사용
    public void UseItem()
    {
        if (item == null 
            || GameManager.instance.playerStateController.CurrentState == PlayerState.Motion 
            || GameManager.instance.playerStateController.CurrentState == PlayerState.Working)
            return;

        switch (item.itemType)
        {
            case ItemData.ItemType.Tent:
                UseTentItem();
                break;
            case ItemData.ItemType.Consumable:
                UseConsumable();
                break;
            case ItemData.ItemType.Book:
                CombinationManager.instance.ToggleRecipeBook();
                break;
            default:
                break;
        }
    }

    // 소비 아이템 사용
    private void UseConsumable()
    {
        GameManager.instance.survivalStatsManager.consumableManager.UseConsumable(item);

        Debug.Log("Used item: " + item.itemName);
        DecreaseQuantity();

        InventoryManager.instance.RemoveItem(item);
        if (quantity <= 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateQuantityText();
        }
    }

    // 텐트 아이템 사용
    private void UseTentItem()
    {
        InventoryUIManager.instance.InventoryUIOff();
        GameManager.instance.tentPlacement.ToggleTentPlacementMode(item);
    }

    // 오른쪽 클릭 처리
    private void HandleRightClick()
    {
        if (item == null)
            return;

        if (InventoryManager.instance.isSellMode)
        {
            ShopManager.instance.SellItem(item);
        }

        else if (isCombinationSlot)
        {
            CombinationSlot emptySlot = CombinationUIManager.instance.combinationSlots.Find(slot => slot.GetItem() == null);
            if (emptySlot != null)
            {
                emptySlot.SetItem(item, this);
                DecreaseQuantity();
                if (quantity == 0)
                    ClearSlot();
                else
                    UpdateQuantityText();
            }
        }
        else if (item.itemType == ItemData.ItemType.Equipment)
        {
            if (InventoryManager.instance.EquipItem(item))
                ClearSlot();
        }
        else
        {
            UseItem();
        }
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null)
            return;

        iconRectTransform = UIManager.instance.MoveIcon(item.icon).GetComponent<RectTransform>();
        iconRectTransform.position = transform.position;
        iconRectTransform.SetAsLastSibling(); // 아이콘을 최상위로 이동
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        if (item == null)
            return;

        iconRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // 마우스를 따라 아이콘 이동
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlot otherSlot = CheckForOtherSlot(eventData);
        if (otherSlot != null)
        {
            MoveItemToSlot(otherSlot);
        }
        else
        {
            if (IsDroppedOutsideUI(eventData) && !isCombinationSlot)
            {
                DropItem(); // 아이템 버리기
            }
            else
            {
                iconRectTransform.anchoredPosition = Vector2.zero; // 원래 위치로 복원
            }
        }
        UIManager.instance.MoveIcon().gameObject.SetActive(false);
    }
    private bool IsDroppedOutsideUI(PointerEventData eventData)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("Inventory"))
            {
                return false;
            }
        }
        return true;
    }


    // 아이템을 다른 슬롯으로 이동
    private void MoveItemToSlot(InventorySlot otherSlot)
    {
        if (otherSlot == null || otherSlot == this)
            return;

        ItemData otherItem = otherSlot.GetItem();
        int otherQuantity = otherSlot.quantity;

        otherSlot.SetItem(this.item, this.quantity);

        if (otherItem != null)
        {
            this.SetItem(otherItem, otherQuantity);
        }
        else
        {
            this.ClearSlot();
        }

        iconRectTransform.anchoredPosition = Vector2.zero; // 원래 위치로 복원
    }

    // 다른 슬롯이 있는지 확인하는 함수
    private InventorySlot CheckForOtherSlot(PointerEventData eventData)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
            if (slot != null && slot != this)
            {
                return slot;
            }
        }
        return null;
    }

    private void DropItem()
    {
        InventoryManager.instance.DropItem(item, quantity);
    }
}
