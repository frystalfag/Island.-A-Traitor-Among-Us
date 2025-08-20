using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    public int maxSlots = 2;
    public List<Item> items = new List<Item>();

    [Header("Drop settings")]
    public float dropForwardDistance = 1.5f;
    public float dropHeight = 1f;

    [Header("Crafting")]
    public CraftManager craftManager;

    private void Awake()
    {
        while (items.Count < maxSlots)
            items.Add(null);

        if (craftManager == null)
            craftManager = FindAnyObjectByType<CraftManager>();
    }

    public void AddItem(Item newItem, int slotIndex)
    {
        if (newItem == null)
        {
            Debug.LogWarning("Попытка добавить null в инвентарь!");
            return;
        }

        if (slotIndex < 0 || slotIndex >= maxSlots)
        {
            Debug.LogWarning("Неверный слот: " + slotIndex);
            return;
        }

        if (items[slotIndex] != null)
        {
            DropItem(slotIndex, transform);
        }

        items[slotIndex] = newItem;
        newItem.gameObject.SetActive(false);

        Rigidbody rb = newItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Debug.Log("Поднял: " + newItem.itemName + " в слот " + slotIndex);
    }

    public void DropItem(int slotIndex, Transform playerTransform)
    {
        if (slotIndex < 0 || slotIndex >= maxSlots) return;

        Item item = items[slotIndex];
        if (item == null) return;

        Vector3 dropPos = playerTransform.position +
                          playerTransform.forward * dropForwardDistance +
                          Vector3.up * dropHeight;

        item.gameObject.SetActive(true);
        item.transform.position = dropPos;

        // костёр поворачиваем особым образом
        if (item.itemName.ToLower().Contains("костёр") || item.itemName.ToLower().Contains("campfire"))
            item.transform.rotation = Quaternion.Euler(-90f, playerTransform.eulerAngles.y, 0f);
        else
            item.transform.rotation = Quaternion.identity;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Debug.Log("Выбросил: " + item.itemName);

        items[slotIndex] = null;

        // проверка крафта
        if (craftManager != null)
            craftManager.CheckCraft(dropPos);
    }
}
