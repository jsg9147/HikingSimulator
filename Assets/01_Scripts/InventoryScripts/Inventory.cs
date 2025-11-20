using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    public bool AddItem(ItemData item, int count = 1)
    {
        if (items.Find(x=> x.itemName == item.itemName) == null && InventoryManager.instance.InventoryIsFull())
        {
            ItemGenerator.instance.DropItemGenerate(item, count);
            Debug.Log("Inventory is full!");
            return false;
        }

        for (int i = 0; i < count; i++)
        {
            ItemData newItem = Instantiate(item);
            items.Add(newItem);
            UIManager.instance.ShowPickupNotification(newItem);
        }
        QuestManager.instance.QuestItemUpdate();
        return true;
    }

    public void RemoveItem(ItemData targetItem)
    {
        ItemData removeItem = targetItem;
        foreach (var item in items)
        {
            if(item.itemName == targetItem.itemName)
            {
                removeItem = item;
            }
        }
        items.Remove(removeItem);
    }

    public int GetItemQuantity(ItemData targetItem)
    {
        int quantity = items.FindAll(x => x.itemName == targetItem.itemName).Count;
        return quantity;
    }

    public ItemData GetItemByName(string itemName)
    {
        return items.Find(x => x.itemName == itemName);
    }
}
