using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "recipeList", menuName = "RecipeList")]
public class RecipeList : ScriptableObject // 조합 레시피 스크립터블 오브젝트
{
    public Recipe[] recipes;
}

[Serializable]
public class Recipe
{
    // public string name;
    public string description;
    public ScriptableItem result;
    public ScriptableItem[] source;
}