using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSlots = 2;
    public List<Item> items = new List<Item>();

    [Header("Drop settings")]
    public float dropForwardDistance = 1.5f;
    public float dropHeight = 1f;

    private void Awake()
    {
        // Инициализация слотов
        while (items.Count < maxSlots)
            items.Add(null);
    }

    // Добавление предмета в конкретный слот
    public void AddItem(Item newItem, int slotIndex)
    {
        if (newItem == null)
        {
            Debug.LogWarning("Попытка добавить null!");
            return;
        }

        if (slotIndex < 0 || slotIndex >= maxSlots)
        {
            Debug.LogWarning("Неверный слот: " + slotIndex);
            return;
        }

        // Если слот занят — выбрасываем старый предмет
        if (items[slotIndex] != null)
        {
            DropItem(slotIndex);
        }

        items[slotIndex] = newItem;
        newItem.gameObject.SetActive(false);

        Rigidbody rb = newItem.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        Debug.Log("Поднял: " + newItem.itemName + " в слот " + slotIndex);
    }

    // Выброс предмета по индексу перед игроком
    public void DropItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSlots) return;

        Item item = items[slotIndex];
        if (item == null) return;

        items[slotIndex] = null;
        item.gameObject.SetActive(true);

        // Позиция спавна перед игроком
        Transform player = transform;
        Vector3 spawnPos = player.position + player.forward * dropForwardDistance + Vector3.up * dropHeight;
        item.transform.position = spawnPos;

        // Кинематика выключена
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        Debug.Log("Выбросил: " + item.itemName + " из слота " + slotIndex);
    }
}
