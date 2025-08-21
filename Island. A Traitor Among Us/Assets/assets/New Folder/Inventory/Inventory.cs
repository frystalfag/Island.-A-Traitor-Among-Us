using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    public int maxSlots = 2;
    public List<Item> items = new List<Item>();
    
    [Header("Drop Settings")]
    public float dropHeight = 1f;

    private void Awake()
    {
        while (items.Count < maxSlots)
            items.Add(null);
    }

    public void AddItem(Item newItem, int slotIndex)
    {
        if (newItem == null) return;
        if (slotIndex < 0 || slotIndex >= maxSlots) return;

        if (items[slotIndex] != null)
            DropItem(slotIndex, transform);

        items[slotIndex] = newItem;
        newItem.gameObject.SetActive(false);
        Rigidbody rb = newItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    public void DropItem(int slotIndex, Transform playerTransform)
    {
        if (slotIndex < 0 || slotIndex >= maxSlots) return;
        Item item = items[slotIndex];
        if (item == null) return;

        item.gameObject.SetActive(true);
        // Изменение: предмет появится на заданной высоте, а затем упадет
        item.transform.position = playerTransform.position + Vector3.up * dropHeight;
        item.transform.rotation = Quaternion.identity;
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        items[slotIndex] = null;
    }
}