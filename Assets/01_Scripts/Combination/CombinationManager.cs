using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : SingletonBase<CombinationManager>
{
    public List<CombinationRecipe> combinationRecipes; // 모든 조합식 리스트
    public Inventory inventory; // 인벤토리 참조

    private RecipeBookManager recipeBookUI;

    public ItemData CombineItemsResult(List<ItemData> items)
    {
        foreach (var recipe in combinationRecipes)
        {
            if (IsMatch(recipe, items))
            {
                return recipe.result;
            }
        }
        return null;
    }

    private bool IsMatch(CombinationRecipe recipe, List<ItemData> items)
    {
        List<ItemData> ingredientsCopy = new List<ItemData>(recipe.ingredients);

        foreach (var item in items)
        {
            bool itemFound = false;
            for (int i = 0; i < ingredientsCopy.Count; i++)
            {
                if (ingredientsCopy[i].itemName == item.itemName && ingredientsCopy[i].isCombinationIngredient)
                {
                    ingredientsCopy.RemoveAt(i);
                    itemFound = true;
                    break;
                }
            }

            if (!itemFound)
            {
                return false;
            }
        }

        // If ingredientsCopy is empty, it means all ingredients matched
        return ingredientsCopy.Count == 0;
    }

    public void DecreaseItemQuantities(List<ItemData> items)
    {
        foreach (var item in items)
        {
            ItemData inventoryItem = inventory.items.Find(i => i.itemName == item.itemName);
            if (inventoryItem != null)
            {
                inventory.RemoveItem(inventoryItem);
            }
        }
    }

    public void SetReceipeBookUI(RecipeBookManager recipeBookManager)
    {
        this.recipeBookUI = recipeBookManager;
        recipeBookUI.SetRecipes(combinationRecipes);
        recipeBookUI.gameObject.SetActive(false);
    }

    public void ToggleRecipeBook()
    {
        if (recipeBookUI != null)
        {
            if (!recipeBookUI.gameObject.activeSelf)
                UIManager.instance.OpenUI(recipeBookUI.gameObject);
            else
                recipeBookUI.gameObject.SetActive(false);
        }
    }
}
