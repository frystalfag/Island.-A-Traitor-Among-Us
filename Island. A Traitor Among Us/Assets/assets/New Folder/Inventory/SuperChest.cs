using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest Settings")]
    [Tooltip("Максимальное количество слотов в сундуке.")]
    public int maxSlots = 9;
    [Tooltip("Текущий список предметов в сундуке.")]
    public List<Item> items = new List<Item>();

    void Awake()
    {
        // Убедимся, что список слотов всегда нужного размера
        while (items.Count < maxSlots)
        {
            items.Add(null);
        }
    }

    // Метод для добавления предмета в сундук
    public bool TryStoreItem(Item itemToAdd)
    {
        if (itemToAdd == null)
        {
            Debug.LogWarning("Попытка добавить null предмет в сундук!");
            return false;
        }

        for (int i = 0; i < maxSlots; i++)
        {
            if (items[i] == null)
            {
                // Помещаем предмет в сундук и отключаем его
                itemToAdd.gameObject.SetActive(false);
                Rigidbody rb = itemToAdd.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
                items[i] = itemToAdd;
                Debug.Log("Добавил " + itemToAdd.itemName + " в сундук.");
                return true;
            }
        }

        Debug.Log("Сундук заполнен.");
        return false;
    }

    // Метод для удаления предмета из сундука
    public Item RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSlots)
        {
            Debug.LogWarning("Неверный индекс слота сундука: " + slotIndex);
            return null;
        }

        Item itemToRemove = items[slotIndex];
        if (itemToRemove != null)
        {
            items[slotIndex] = null;
            Debug.Log("Удалил " + itemToRemove.itemName + " из сундука.");
        }
        return itemToRemove;
    }
}