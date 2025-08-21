using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // === Inventory Settings ===
    [Header("Inventory")]
    [Tooltip("Максимальное количество слотов в инвентаре.")]
    public int maxSlots = 2;
    [Tooltip("Список предметов в инвентаре.")]
    public List<Item> items = new List<Item>();

    // === Drop Settings ===
    [Header("Drop settings")]
    [Tooltip("Расстояние, на которое предмет будет выброшен вперед.")]
    public float dropForwardDistance = 1.5f;
    [Tooltip("Высота, на которой предмет будет выброшен.")]
    public float dropHeight = 1f;

    // === Crafting ===
    [Header("Crafting")]
    [Tooltip("Ссылка на скрипт CraftManager.")]
    public CraftManager craftManager;

    private void Awake()
    {
        if (items == null)
        {
            items = new List<Item>();
        }
        
        while (items.Count < maxSlots)
            items.Add(null);

        if (craftManager == null)
            craftManager = FindAnyObjectByType<CraftManager>();
        
        if (craftManager == null)
        {
            Debug.LogWarning("CraftManager не найден на сцене. Крафт работать не будет.");
        }
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
        
        if (items == null)
        {
            Debug.LogError("Список предметов инвентаря не инициализирован!");
            return;
        }

        if (items[slotIndex] != null)
        {
            DropItem(slotIndex, transform);
        }

        newItem.gameObject.SetActive(false);
        items[slotIndex] = newItem;

        Rigidbody rb = newItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void DropItem(int slotIndex, Transform playerTransform)
    {
        if (slotIndex < 0 || slotIndex >= maxSlots) return;

        if (items == null || items.Count <= slotIndex)
        {
            Debug.LogError("Список предметов инвентаря не инициализирован или имеет неверный размер!");
            return;
        }
        
        Item item = items[slotIndex];
        if (item == null)
        {
            Debug.Log("В слоте " + slotIndex + " ничего нет.");
            return;
        }

        items[slotIndex] = null;

        if (item.gameObject == null)
        {
            Debug.LogError("Игровой объект предмета оказался null!");
            return;
        }

        item.gameObject.SetActive(true);

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform не подключен для выброса предмета!");
            return;
        }

        Vector3 dropPos = playerTransform.position + playerTransform.forward * dropForwardDistance + Vector3.up * dropHeight;

        RaycastHit hit;
        int groundLayer = LayerMask.GetMask("Ground");
        if (Physics.Raycast(dropPos, Vector3.down, out hit, 5f, groundLayer))
            dropPos.y = hit.point.y + 0.1f;

        item.transform.position = dropPos;

        if (item.itemName.ToLower().Contains("костёр") || item.itemName.ToLower().Contains("campfire"))
            item.transform.rotation = Quaternion.Euler(-90f, playerTransform.eulerAngles.y, 0f);
        else
            item.transform.rotation = Quaternion.identity;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Кинематика всегда включена
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (craftManager != null)
        {
            craftManager.CheckCraft(dropPos);
        }
    }
}