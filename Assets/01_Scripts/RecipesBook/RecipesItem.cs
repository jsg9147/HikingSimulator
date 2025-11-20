using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipesItem : MonoBehaviour
{
    public Sprite plusSprite;
    public Sprite equalSprite;

    public Image iconPrefab;
    public Transform iconParent; // 생성된 아이콘들을 담을 부모 트랜스폼

    private CombinationRecipe recipes;

    public void SetRecipes(CombinationRecipe combinationRecipe)
    {
        // 기존의 아이콘들을 모두 제거
        foreach (Transform child in iconParent)
        {
            Destroy(child.gameObject);
        }

        recipes = combinationRecipe;

        // 재료들을 순회하며 아이콘을 추가
        for (int i = 0; i < recipes.ingredients.Count; i++)
        {
            // 재료 아이콘 생성 및 설정
            Image ingredientIcon = Instantiate(iconPrefab, iconParent);
            ingredientIcon.sprite = recipes.ingredients[i].icon;

            // 마지막 재료를 제외한 모든 재료 뒤에 더하기 아이콘 추가
            if (i < recipes.ingredients.Count - 1)
            {
                Image plusIcon = Instantiate(iconPrefab, iconParent);
                plusIcon.sprite = plusSprite;
            }
        }

        // 등호 아이콘 추가
        Image equalIcon = Instantiate(iconPrefab, iconParent);
        equalIcon.sprite = equalSprite;

        // 결과 아이콘 추가
        Image resultIcon = Instantiate(iconPrefab, iconParent);
        resultIcon.sprite = recipes.result.icon;
    }
}
