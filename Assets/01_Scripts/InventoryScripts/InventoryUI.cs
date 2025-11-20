using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    public List<BackpackSlot> backPackSlots;
    public EquipmentSlot[] equipmentSlots; // 장비 슬롯 배열
    public List<InventorySlot> inventorySlots; // 인벤토리 슬롯 배열

    int currentBackpackSize = 0;

    public Dictionary<string, int> itemPosInfo = new Dictionary<string, int>(); // itemName을 키로 사용

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetItemPosInfo(Dictionary<string, int> itemPosInfo)
    {
        this.itemPosInfo = itemPosInfo;
        SyncSlotsWithInventory();
    }

    public void SlotUpdate(int backpackSize)
    {
        foreach (var slot in backPackSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < backpackSize; i++)
        {
            if (backPackSlots.Count > i)
            {
                backPackSlots[i].gameObject.SetActive(true);
            }
        }
        currentBackpackSize = backpackSize;
    }

    //public void AddItem(ItemData item, int quantity = 1)
    //{
    //    string itemName = item.itemName; // itemName을 사용하여 중복 확인

    //    // 인벤토리 슬롯에 추가 시도
    //    foreach (var slot in inventorySlots)
    //    {
    //        if (slot.GetItem() == null || slot.GetItem().itemName == itemName)
    //        {
    //            slot.SetItem(item, slot.GetItem() == null ? quantity : slot.GetQuantity() + quantity);
    //            return;
    //        }
    //    }

    //    // 인벤토리 슬롯이 가득 찬 경우 백팩 슬롯에 추가 시도
    //    foreach (var backpackSlot in backPackSlots)
    //    {
    //        foreach (var slot in backpackSlot.slots)
    //        {
    //            if (slot.GetItem() == null || slot.GetItem().itemName == itemName)
    //            {
    //                slot.SetItem(item, slot.GetItem() == null ? quantity : slot.GetQuantity() + quantity);
    //                return;
    //            }
    //        }
    //    }

    //    // 인벤토리와 백팩 슬롯이 모두 가득 찬 경우 처리 (예: 경고 메시지)
    //    Debug.LogWarning("Inventory and backpack slots are full!");
    //}

    public void SyncSlotsWithInventory()
    {
        // 모든 슬롯 가져오기
        List<InventorySlot> allSlots = InventorySlots();
        Inventory inventory = InventoryManager.instance.GetInventory();

        // 슬롯 초기화
        foreach (var slot in allSlots)
        {
            slot.ClearSlot();
        }

        if (itemPosInfo != null)
        {
            foreach (var itemEntry in itemPosInfo)
            {
                string itemName = itemEntry.Key;
                int index = itemEntry.Value;
                if (index >= 0 && index < allSlots.Count)
                {
                    ItemData item = inventory.GetItemByName(itemName); // itemName으로 아이템 찾기
                    if (item != null)
                    {
                        allSlots[index].SetItem(item, inventory.GetItemQuantity(item));
                    }
                }
            }
        }

        // 인벤토리에 남은 아이템들을 빈 슬롯에 채우기
        foreach (var item in inventory.items)
        {
            if (itemPosInfo == null || !itemPosInfo.ContainsKey(item.itemName))
            {
                foreach (var slot in allSlots)
                {
                    if (slot.GetItem() == null)
                    {
                        slot.SetItem(item, inventory.GetItemQuantity(item));
                        break;
                    }
                }
            }
        }
    }

    public void SaveItemIndex()
    {
        List<InventorySlot> allSlots = InventorySlots();
        itemPosInfo = new Dictionary<string, int>(); // itemName을 키로 사용
        for (int i = 0; i < allSlots.Count; i++)
        {
            InventorySlot slot = allSlots[i];
            if (slot != null && slot.GetItem() != null)
            {
                itemPosInfo[slot.GetItem().itemName] = i;
            }
        }
        InventoryUIManager.instance.SetInventoryItemPos(itemPosInfo);
    }

    public List<InventorySlot> InventorySlots()
    {
        List<InventorySlot> totalSlots = new(inventorySlots);
        for (int i = 0; i < currentBackpackSize; i++)
        {
            if (backPackSlots.Count > i)
            {
                foreach (InventorySlot inventorySlot in backPackSlots[i].slots)
                {
                    totalSlots.Add(inventorySlot);
                }
            }
        }
        return totalSlots;
    }

    public void OnDisable()
    {
        InventoryUIManager.instance.DescriptionPopupOff();
    }
}
