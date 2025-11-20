using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using DarkTonic.MasterAudio;

public class InventoryManager : SingletonBase<InventoryManager>
{
    public ItemData startingBackpack; // 시작 장비 (가방)
    public ItemData startTent; // 시작 장비 (가방)
    public int startingCurrency = 10000; // 시작 재화

    [Header("References")]
    public Inventory inventory;
    public CurrencyManager currencyManager;
    public EquipmentManager equipmentManager;
    public DraggedItem draggedItemIconPrefab;

    private List<InventorySlot> inventorySlots; // 인벤토리 슬롯 배열
    public EquipmentSlot[] equipmentSlots; // 장비 슬롯 배열

    public bool isSellMode = false;

    //DraggedItem draggedItemIcon;
    InventorySlot dragSlot;

    public InventorySlot DragSlot => dragSlot;

    protected override void Awake()
    {
        base.Awake();
    }

    public void InitializeManagers()
    {
        if (inventory == null || currencyManager == null || equipmentManager == null)
        {
            Debug.LogError("Required components are missing from InventoryManager.");
        }

        InitializeInventoryUI();
        UpdateCurrencyData();
        equipmentManager.SetEquipmentSlots(InventoryUIManager.instance.InventoryUI.equipmentSlots);
        SetInventorySlotCount();
        SetEquipmentSlots(InventoryUIManager.instance.InventoryUI.equipmentSlots);
        
    }

    void Start()
    {
        AddStartingItems();
    }

    public void SetInventorySlots(List<InventorySlot> slots)
    {
        inventorySlots = slots;
    }

    public void SetEquipmentSlots(EquipmentSlot[] slots)
    {
        equipmentSlots = slots;
        equipmentManager.SetEquipmentSlots(slots);
    }


    public Inventory GetInventory()
    {
        return inventory;
    }

    public bool InventoryIsFull()
    {
        List<ItemData> items = new(); 
        foreach (var item in inventory.items)
        {
            if (items.Find(x => x.itemName == item.itemName) == null)
            {
                items.Add(item);
            }
        }
        return GetInventorySlotCount() <= items.Count;
    }

    public bool SearchItem(ItemData targetItem)
    {
        foreach (var item in inventory.items)
        {
            if(item.itemName == targetItem.itemName) return true;
        }
        return false;
    }

    public int GetInventorySlotCount()
    {
        return InventoryUIManager.instance.InventoryUI.InventorySlots().Count;
    }

    public List<InventorySlot> GetInventorySlots()
    {
        return inventorySlots;
    }

    public void SetInventorySlotCount()
    {
        ItemData backPackData = equipmentManager.GetEquippedItem(ItemData.EquipmentType.Backpack);
        int backPackSize = 0;
        if (backPackData != null)
        {
            backPackSize = backPackData.slotIncreaseAmount;
        }
        InventoryUIManager.instance.SetInventorySlots(backPackSize);
    }

    public void UpdateInventoryUI()
    {
        InventoryUIManager.instance.UpdateInventoryUI();
    }

    public int GetUniqueItemCount()
    {
        return inventory.items.Count;
    }

    public int GetItemCount(ItemData targetItem)
    {
        List<ItemData> targetList = inventory.items.FindAll(x => x.itemName == targetItem.itemName);

        return targetList.Count;
    }

    public void UnequipItem(ItemData item)
    {
        equipmentManager.UnequipItem(item);
        if (item.equipmentType == ItemData.EquipmentType.Backpack)
        {
            SetInventorySlotCount();
        }
        InventoryUIManager.instance.UpdateInventoryUI();
    }

    public bool EquipItem(ItemData item)
    {
        if (item.equipmentType == ItemData.EquipmentType.Backpack)
        {
            ItemData currentBackpack = equipmentManager.GetEquippedItem(ItemData.EquipmentType.Backpack);
            if (currentBackpack != null && item.slotIncreaseAmount <= currentBackpack.slotIncreaseAmount)
            {
                Debug.LogWarning("Cannot equip a backpack with fewer or equal slots.");
                return false;
            }
        }

        equipmentManager.EquipItem(item);
        SetInventorySlotCount();
        InventoryUIManager.instance.UpdateInventoryUI();
        MasterAudio.PlaySound3DAtTransform("EquipmentChange", GameManager.instance.playerTransfrom);
        return true;
    }

    public void AddCurrency(int amount)
    {
        currencyManager.AddCurrency(amount);
    }

    public void RemoveCurrency(int amount)
    {
        currencyManager.RemoveCurrency(amount);
    }

    public int GetCurrency() { return currencyManager.GetCurrency(); }

    public void ShowPopup(Vector3 position, UnityAction useAction, UnityAction removeAction)
    {
        InventoryUIManager.instance.ShowPopup(position, useAction, removeAction);
    }

    public void HidePopup()
    {
        InventoryUIManager.instance.HidePopup();
    }

    public void InitializeInventoryUI()
    {
        InventoryUIManager.instance.InitializeInventoryUI();
    }

    public void UpdateCurrencyData()
    {
        currencyManager.UpdateCurrencyUI();
    }
    private void AddStartingItems()
    {
        if (startingBackpack != null)
        {
            SetEquipmentSlots(InventoryUIManager.instance.InventoryUI.equipmentSlots);
            equipmentManager.EquipItem(startingBackpack);
            SetInventorySlotCount();
        }
        if (startTent != null)
        {
            ItemData tent = Instantiate(startTent);
            AddItem(tent);
        }
        currencyManager.InitializeCurrency(startingCurrency);
    }

    public bool AddItem(ItemData item)
    {
        if (inventory.AddItem(item))
        {
            InventoryUIManager.instance.UpdateInventoryUI();
            QuestManager.instance.QuestItemUpdate();
            return true;
        }
        else
        {
            Debug.LogWarning($"Failed to add item {item.itemName} to inventory.");
            return false;
        }
    }

    public void AddItem(List<ItemData> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (inventory.AddItem(items[i]))
            {
                InventoryUIManager.instance.UpdateInventoryUI();
                QuestManager.instance.QuestItemUpdate();
            }
            else
            {
                ItemGenerator.instance.DropItemGenerate(items[i], 1);
            }
        }
    }

    public void RemoveItem(ItemData item, int removeCount = 1)
    {
        for (int i = 0; i < removeCount; i++)
        {
            inventory.RemoveItem(item);
        }
        InventoryUIManager.instance.UpdateInventoryUI();
    }

    public void DropItem(ItemData item, int count)
    {
        RemoveItem(item, count);
        ItemGenerator.instance.DropItemGenerate(item, count);
    }

    public void MoveItem(InventorySlot slot)
    {
        if (dragSlot == null)
            return;

        if (slot.GetItem() == null)
        {
            slot.SetItem(dragSlot.GetItem());
            dragSlot.ClearSlot();
        }
        else
        {
            ItemData tempData = dragSlot.GetItem();
            dragSlot.SetItem(slot.GetItem());
            slot.SetItem(tempData);
        }
        dragSlot = null;
        UpdateInventoryUI();
    }
}