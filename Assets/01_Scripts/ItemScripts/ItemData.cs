using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemType itemType;
    public EquipmentType equipmentType;
    public int price;
    public int healthRestore;
    public int hungerRestore;
    public int slotIncreaseAmount; // 가방이 제공하는 추가 슬롯 수량
    public bool isCombinationIngredient; // 조합 재료로 사용 가능한지 여부
    public bool isStackable  = true; 

    // 부상 치료 속성
    public bool healsMinorInjury; // 경상을 치료하는지 여부
    public bool healsMajorInjury; // 중상을 치료하는지 여부

    public enum ItemType
    {
        Consumable,
        Equipment,
        ETC,
        Tent,
        Book
    }

    public enum EquipmentType
    {
        None,
        Backpack,
        Clothes,
        Shoes
    }
}
