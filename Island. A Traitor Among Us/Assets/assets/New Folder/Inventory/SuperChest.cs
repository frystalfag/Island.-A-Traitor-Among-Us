using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest Settings")]
    public int maxSlots = 9;
    public float pickRadius = 2f;
    public List<Item> initialItems = new List<Item>();

    [HideInInspector]
    public List<Item> storedItems = new List<Item>();

    private void Awake()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            storedItems.Add(null);
        }

        for (int i = 0; i < initialItems.Count && i < maxSlots; i++)
        {
            if (initialItems[i] != null)
            {
                Item newItem = Instantiate(initialItems[i], transform);
                newItem.gameObject.SetActive(false);
                storedItems[i] = newItem;
            }
        }
    }
    
    private void Update()
    {
        // Возвращаем вызов, чтобы сундук постоянно собирал предметы
        CollectNearbyItems();
    }

    public void CollectNearbyItems()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickRadius);
        foreach (Collider col in colliders)
        {
            Item item = col.GetComponent<Item>();
            if (item != null && item.gameObject.activeInHierarchy)
                TryStoreItem(item);
        }
    }

    public bool TryStoreItem(Item item)
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (storedItems[i] == null)
            {
                storedItems[i] = item;
                item.gameObject.SetActive(false);
                return true;
            }
        }
        return false;
    }

    public Item TakeItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxSlots)
            return null;

        Item item = storedItems[slotIndex];
        storedItems[slotIndex] = null;
        return item;
    }
}