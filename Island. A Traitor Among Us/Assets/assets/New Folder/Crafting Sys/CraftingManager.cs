using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public List<CraftRecipe> recipes;
    public float craftRadius = 2f;
    public string groundLayerName = "Ground";

    public void CheckCraft(Vector3 dropPosition)
    {
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

        Debug.Log("Предметы вокруг дропа:");
        foreach (Item it in nearbyItems)
            Debug.Log("- " + it.itemName);

        foreach (CraftRecipe recipe in recipes)
        {
            bool canCraft = true;

            for (int i = 0; i < recipe.requiredItemNames.Count; i++)
            {
                string reqName = recipe.requiredItemNames[i];
                int need = recipe.requiredAmounts[i];
                int count = 0;

                foreach (Item it in nearbyItems)
                    if (it.itemName == reqName) count++;

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
                        if (it.itemName == reqName && removed < need)
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

                    if (Physics.Raycast(spawnPos, Vector3.down, out hit, 10f, groundLayer))
                        spawnPos.y = hit.point.y + 0.05f;

                    Quaternion spawnRotation = Quaternion.identity;
                    if (recipe.resultPrefab.name.ToLower().Contains("campfire"))
                        spawnRotation = Quaternion.Euler(-90f, 0f, 0f);

                    Instantiate(recipe.resultPrefab, spawnPos, spawnRotation);
                    Debug.Log("Создан объект: " + recipe.resultPrefab.name + " на позиции " + spawnPos);
                }

                break;
            }
        }
    }
}