using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCombinationRecipe", menuName = "Inventory/CombinationRecipe")]
public class CombinationRecipe : ScriptableObject
{
    public List<ItemData> ingredients; // 조합에 필요한 아이템 리스트
    public ItemData result; // 조합 결과 아이템
}
