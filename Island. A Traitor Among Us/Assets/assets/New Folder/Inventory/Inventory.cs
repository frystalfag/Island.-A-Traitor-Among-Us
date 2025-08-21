using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // === Inventory Settings ===
    [Header("Inventory")]
    public int maxSlots = 2;
    public List<Item> items = new List<Item>();

    // === Drop Settings ===
    [Header("Drop settings")]
    public float dropForwardDistance = 1.5f;
    public float dropHeight = 1f;

    // === Crafting ===
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

        Item item = items[slotIndex];
        if (item == null)
        {
            Debug.Log("В слоте " + slotIndex + " ничего нет.");
            return;
        }

        items[slotIndex] = null;
        item.gameObject.SetActive(true);

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
            rb.isKinematic = true; // Теперь всегда true, как ты просил
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (craftManager != null)
        {
            craftManager.CheckCraft(dropPos);
        }
    }
}