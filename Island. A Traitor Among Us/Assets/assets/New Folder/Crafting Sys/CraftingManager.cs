using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public List<CraftRecipe> recipes;      // Список всех рецептов
    public float craftRadius = 2f;         // Радиус проверки вокруг дропнутого предмета
    public string groundLayerName = "Ground"; // Слой пола

    public void CheckCraft(Vector3 dropPosition)
    {
        // Проверяем все коллайдеры вокруг
        Collider[] colliders = Physics.OverlapSphere(dropPosition, craftRadius);
        List<Item> nearbyItems = new List<Item>();

        foreach (Collider col in colliders)
        {
            Item it = col.GetComponent<Item>();
            if (it != null && it.gameObject.activeInHierarchy)
                nearbyItems.Add(it);
        }

        if (nearbyItems.Count == 0)
        {
            Debug.Log("Рядом с предметом ничего нет.");
            return;
        }

        // Выводим предметы вокруг дропа
        Debug.Log("Предметы вокруг дропа:");
        foreach (Item it in nearbyItems)
            Debug.Log("- " + it.itemName);

        // Проверяем все рецепты
        foreach (CraftRecipe recipe in recipes)
        {
            bool canCraft = true;

            for (int i = 0; i < recipe.requiredItemNames.Count; i++)
            {
                string reqName = recipe.requiredItemNames[i];
                int need = recipe.requiredAmounts[i];
                int count = 0;

                foreach (Item it in nearbyItems)
                    if (it.itemName == reqName)
                        count++;

                if (count < need)
                {
                    canCraft = false;
                    break;
                }
            }

            if (canCraft)
            {
                // Убираем использованные предметы
                for (int i = 0; i < recipe.requiredItemNames.Count; i++)
                {
                    string reqName = recipe.requiredItemNames[i];
                    int need = recipe.requiredAmounts[i];
                    int removed = 0;

                    foreach (Item it in new List<Item>(nearbyItems))
                    {
                        if (it.itemName == reqName && removed < need)
                        {
                            it.gameObject.SetActive(false);
                            nearbyItems.Remove(it);
                            removed++;
                        }
                    }
                }

                // Создаём результат на полу
                if (recipe.resultPrefab != null)
                {
                    Vector3 spawnPos = dropPosition + Vector3.up * 2f; // стартовая высота для Raycast
                    RaycastHit hit;
                    int groundLayer = LayerMask.GetMask(groundLayerName);
                    if (Physics.Raycast(spawnPos, Vector3.down, out hit, 10f, groundLayer))
                        spawnPos.y = hit.point.y + 0.05f; // чуть выше пола

                    Quaternion uprightRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
		    Instantiate(recipe.resultPrefab, spawnPos, uprightRotation);
                    Debug.Log("Создан объект: " + recipe.resultPrefab.name + " на позиции " + spawnPos);
                }

                break; // только один рецепт за раз
            }
        }
    }
}