using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text itemNameText; // 아이템 이름 텍스트
    public TMP_Text itemPriceText; // 아이템 가격 텍스트
    public Image itemIconImage; // 아이템 아이콘 이미지
    public Button buyButton; // 구매 버튼

    private ItemData shopItem; // 현재 아이템
    private ShopManager shopManager; // 상점 관리자

    // 아이템 UI를 설정하는 함수
    public void SetItem(ItemData newItem, ShopManager manager)
    {
        shopItem = newItem;
        shopManager = manager;

        itemNameText.text = shopItem.itemName;
        itemPriceText.text = shopItem.price.ToString("N0") + " $";
        itemIconImage.sprite = shopItem.icon;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => shopManager.BuyItem(shopItem));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryUIManager.instance.DescriptionPopupOn(shopItem);
    }

    // 마우스가 슬롯을 떠났을 때 처리
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUIManager.instance.DescriptionPopupOff();
    }
}
