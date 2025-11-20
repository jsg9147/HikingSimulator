using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBookManager : SingletonBase<RecipeBookManager>
{
    public RecipesItem recipesItemPrefab;
    public Transform recipesParent;

    public void SetRecipes(List<CombinationRecipe> combinationRecipes)
    {
        for (int i = 0; i < combinationRecipes.Count; i++)
        {
            RecipesItem recipesItem = Instantiate(recipesItemPrefab, recipesParent);
            recipesItem.SetRecipes(combinationRecipes[i]);
        }
    }
}
