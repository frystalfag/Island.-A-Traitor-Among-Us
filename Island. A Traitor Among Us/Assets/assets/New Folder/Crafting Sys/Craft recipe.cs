using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftRecipe
{
    public string recipeName;
    public List<string> requiredItemNames;
    public List<int> requiredAmounts;
    public GameObject resultPrefab;
}
