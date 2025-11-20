using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    public List<ItemData> shopItems; // 상점의 아이템 목록
    public GameObject shopItemPrefab; // 아이템을 표시할 프리팹
    public ShopUI shopUIprefab; // 상점 UI
    public ShopUI shopUI;

    public float resellPriceRate = 0.7f;

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

    void Start()
    {
        ShopInit();
    }

    public void ShopInit()
    {
        shopUI = Instantiate(shopUIprefab);
        shopUI.transform.SetParent(GameObject.Find("Canvas").transform, false);
        shopUI.gameObject.SetActive(false);
        UpdateShopUI();
    }

    void UpdateShopUI()
    {
        foreach (Transform child in shopUI.itemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var shopItem in shopItems)
        {
            GameObject itemGO = Instantiate(shopItemPrefab, shopUI.itemContainer);
            ShopItemUI itemUI = itemGO.GetComponent<ShopItemUI>();
            itemUI.SetItem(shopItem, this);
        }
    }

    public void BuyItem(ItemData shopItem)
    {
        if (InventoryManager.instance.inventory.items.Find(x => x.itemName == shopItem.itemName) == null)
        {
            if (InventoryManager.instance.InventoryIsFull())
            {
                Debug.Log("Inventory is full");
                return;
            }
        }
        if (InventoryManager.instance.GetCurrency() >= shopItem.price)
        {
            InventoryManager.instance.RemoveCurrency(shopItem.price);
            InventoryManager.instance.AddItem(shopItem);
            InventoryManager.instance.UpdateCurrencyData();
            MasterAudio.PlaySound3DAtTransform("ItemPurchase", GameManager.instance.playerTransfrom);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    // 상점 UI를 활성화하는 메서드
    public void OpenShop()
    {
        UIManager.instance.OpenUI(shopUI.gameObject);
    }

    // 상점 UI를 비활성화하는 메서드
    public void CloseShop()
    {
        shopUI.gameObject.SetActive(false);
        UIManager.instance.CursorChange(CursorLockMode.Locked);
    }

    // 외부에서 shopItems를 설정할 수 있는 메서드
    public void SetShopItems(List<ItemData> newShopItems)
    {
        shopItems = newShopItems;
        UpdateShopUI();
    }

    public void SellItem(ItemData itemData)
    {
        InventoryManager.instance.AddCurrency(Mathf.FloorToInt(itemData.price * resellPriceRate));
        InventoryManager.instance.RemoveItem(itemData);
        InventoryManager.instance.UpdateCurrencyData();
        InventoryManager.instance.UpdateInventoryUI();
    }
}
