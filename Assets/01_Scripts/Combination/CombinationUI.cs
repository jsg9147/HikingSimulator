using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombinationUI : MonoBehaviour
{
    public List<CombinationSlot> slots; // 조합 슬롯 목록
    public Button combinationBtn; // 조합 버튼

    public TMP_Text countText; // 아이템 수량 텍스트
    public List<InventorySlot> inventorySlots; // 인벤토리 슬롯 목록

    private void OnEnable()
    {
        SetupIngredientItems();
    }

    private void OnDisable()
    {
        ClearCombinationSlots();
        InventoryUIManager.instance.UpdateInventoryUI();
    }

    /// <summary>
    /// 인벤토리 슬롯을 초기화하고, 조합 재료 아이템을 설정합니다.
    /// </summary>
    public void SetupIngredientItems()
    {
        ClearInventorySlots();

        List<ItemData> items = InventoryManager.instance.inventory.items;
        int itemCount = 0;

        foreach (var item in items)
        {
            if (item.isCombinationIngredient)
            {
                bool itemAdded = TryAddItemToExistingSlot(item);

                if (!itemAdded)
                {
                    itemCount += AddItemToEmptySlot(item);
                }
            }
        }

        UpdateItemCountText(itemCount);
    }

    /// <summary>
    /// 인벤토리 슬롯을 초기화합니다.
    /// </summary>
    private void ClearInventorySlots()
    {
        foreach (var slot in inventorySlots)
        {
            slot.ClearSlot();
        }
    }

    /// <summary>
    /// 기존 슬롯에 아이템을 추가하려고 시도합니다.
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    /// <returns>아이템이 추가되었는지 여부</returns>
    private bool TryAddItemToExistingSlot(ItemData item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.GetItem() != null && slot.GetItem().itemName == item.itemName && slot.GetItem().isStackable)
            {
                slot.IncreaseQuantity();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 빈 슬롯에 아이템을 추가합니다.
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    /// <returns>추가된 아이템 수량</returns>
    private int AddItemToEmptySlot(ItemData item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.GetItem() == null)
            {
                slot.SetItem(item);
                return 1;
            }
        }
        return 0;
    }

    /// <summary>
    /// 아이템 수량 텍스트를 업데이트합니다.
    /// </summary>
    /// <param name="itemCount">현재 아이템 수량</param>
    private void UpdateItemCountText(int itemCount)
    {
        countText.text = $"<color=#F8913F>{itemCount}</color> / {inventorySlots.Count}";
    }

    /// <summary>
    /// 조합 슬롯을 초기화합니다.
    /// </summary>
    private void ClearCombinationSlots()
    {
        foreach (var slot in slots)
        {
            if (slot != null && slot.GetItem() != null)
            {
                // 조합 슬롯의 아이템을 인벤토리로 돌려주는 코드
                // InventoryManager.instance.GetInventory().AddItem(slot.GetItem());
                slot.ClearSlot();
            }
        }
    }
}
