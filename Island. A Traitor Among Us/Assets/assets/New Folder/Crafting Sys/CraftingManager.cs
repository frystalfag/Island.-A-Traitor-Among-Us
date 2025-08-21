using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [Tooltip("Список рецептов крафта.")]
    public List<CraftRecipe> recipes;
    [Tooltip("Радиус, в котором проверяются предметы для крафта.")]
    public float craftRadius = 2f;
    [Tooltip("Название слоя 'земля'.")]
    public string groundLayerName = "Ground";

    public void CheckCraft(Vector3 dropPosition)
    {
        // Проверка на наличие рецептов
        if (recipes == null || recipes.Count == 0)
        {
            Debug.LogWarning("Список рецептов пуст! Крафт невозможен.");
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(dropPosition, craftRadius);
        List<Item> nearbyItems = new List<Item>();

        foreach (Collider col in colliders)
        {
            Item it = col.GetComponent<Item>();
            if (it != null && it.gameObject != null && it.gameObject.activeInHierarchy)
                nearbyItems.Add(it);
        }

        if (nearbyItems.Count == 0)
        {
            Debug.Log("Рядом с предметом ничего нет.");
            return;
        }

        Debug.Log("Предметы вокруг дропа:");
        foreach (Item it in nearbyItems)
        {
            if (it != null)
                Debug.Log("- " + it.itemName);
        }

        foreach (CraftRecipe recipe in recipes)
        {
            if (recipe == null || recipe.requiredItemNames == null || recipe.requiredAmounts == null)
            {
                Debug.LogWarning("Один из рецептов крафта не настроен правильно. Проверьте инспектор.");
                continue;
            }

            bool canCraft = true;

            for (int i = 0; i < recipe.requiredItemNames.Count; i++)
            {
                string reqName = recipe.requiredItemNames[i];
                int need = recipe.requiredAmounts[i];
                int count = 0;

                foreach (Item it in nearbyItems)
                {
                    if (it != null && it.itemName == reqName) count++;
                }

                if (count < need)
                {
                    canCraft = false;
                    break;
                }
            }

            if (canCraft)
            {
                for (int i = 0; i < recipe.requiredItemNames.Count; i++)
                {
                    string reqName = recipe.requiredItemNames[i];
                    int need = recipe.requiredAmounts[i];
                    int removed = 0;

                    foreach (Item it in new List<Item>(nearbyItems))
                    {
                        if (it != null && it.gameObject != null && it.itemName == reqName && removed < need)
                        {
                            it.gameObject.SetActive(false);
                            nearbyItems.Remove(it);
                            removed++;
                        }
                    }
                }

                if (recipe.resultPrefab != null)
                {
                    Vector3 spawnPos = dropPosition + Vector3.up * 2f;
                    RaycastHit hit;
                    int groundLayer = LayerMask.GetMask(groundLayerName);

                    if (groundLayer != 0 && Physics.Raycast(spawnPos, Vector3.down, out hit, 10f, groundLayer))
                        spawnPos.y = hit.point.y + 0.05f;

                    Quaternion spawnRotation = Quaternion.identity;
                    if (recipe.resultPrefab.name.ToLower().Contains("campfire") || recipe.resultPrefab.name.ToLower().Contains("костёр"))
                        spawnRotation = Quaternion.Euler(-90f, 0f, 0f);

                    Instantiate(recipe.resultPrefab, spawnPos, spawnRotation);
                    Debug.Log("Создан объект: " + recipe.resultPrefab.name + " на позиции " + spawnPos);
                }
                else
                {
                    Debug.LogWarning("Результат крафта в рецепте '" + recipe.recipeName + "' не указан!");
                }

                break;
            }
        }
    }
}