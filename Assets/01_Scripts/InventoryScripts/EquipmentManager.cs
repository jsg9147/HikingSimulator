using UnityEngine;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance; // ΩÃ±€≈Ê ¿ŒΩ∫≈œΩ∫
    private Dictionary<ItemData.EquipmentType, ItemData> equippedItems = new Dictionary<ItemData.EquipmentType, ItemData>();
    public EquipmentSlot[] equipmentSlots; // ¿Â∫Ò ΩΩ∑‘ πËø≠

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EquipItem(ItemData item)
    {
        if (item.itemType == ItemData.ItemType.Equipment)
        {
            var equipmentType = item.equipmentType;
            ItemData currentBackpack = null;
            ItemData unequippedItem = null;

            foreach (var slot in equipmentSlots)
            {
                if (slot.equipmentType == equipmentType)
                {
                    if (slot.GetItem() != null)
                    {
                        unequippedItem = slot.GetItem();
                        if (equipmentType == ItemData.EquipmentType.Backpack)
                        {
                            currentBackpack = unequippedItem;
                        }
                        slot.ClearSlot();
                    }
                    slot.SetItem(item);
                    break;
                }
            }

            if (unequippedItem != null)
            {
                if (InventoryManager.instance.InventoryIsFull())
                {
                    Debug.Log("Inventory is full! Cannot unequip " + unequippedItem.itemName);
                    foreach (var slot in equipmentSlots)
                    {
                        if (slot.equipmentType == equipmentType && slot.GetItem() == null)
                        {
                            slot.SetItem(unequippedItem);
                            break;
                        }
                    }
                }
            }

            InventoryManager.instance.GetInventory().RemoveItem(item);
            equippedItems[equipmentType] = item;
        }
        InventoryManager.instance.UpdateInventoryUI();
    }

    public void UnequipItem(ItemData item)
    {
        if (item.equipmentType == ItemData.EquipmentType.Backpack)
        {
            InventoryManager.instance.SetInventorySlotCount();
        }

        if (InventoryManager.instance.GetInventory().items.Count < InventoryManager.instance.GetInventorySlotCount())
        {
            foreach (var slot in equipmentSlots)
            {
                if (slot.GetItem() != null && slot.GetItem().itemName == item.itemName)
                {
                    slot.ClearSlot();
                    InventoryManager.instance.GetInventory().AddItem(item);
                    equippedItems.Remove(item.equipmentType);
                    InventoryManager.instance.UpdateInventoryUI();
                    return;
                }
            }
        }
        else
        {
            Debug.Log("Inventory is full! Cannot unequip " + item.itemName);
        }
    }

    public void SetEquipmentSlots(EquipmentSlot[] slots)
    {
        equipmentSlots = slots;
    }

    public void RestoreEquippedItems()
    {
        foreach (var kvp in equippedItems)
        {
            foreach (var slot in equipmentSlots)
            {
                if (slot.equipmentType == kvp.Key)
                {
                    slot.SetItem(kvp.Value);
                    break;
                }
            }
        }
    }

    public ItemData GetEquippedItem(ItemData.EquipmentType equipmentType)
    {
        if (equippedItems.ContainsKey(equipmentType))
        {
            return equippedItems[equipmentType];
        }
        return null;
    }
}
