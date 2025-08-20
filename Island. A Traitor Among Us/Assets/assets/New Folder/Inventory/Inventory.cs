using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSlots = 2;
    public List<Item> items = new List<Item>();

    public float dropForwardDistance = 1.5f;
    public float dropHeight = 0.5f;

    private void Awake()
    {
        // Убедимся, что список слотов всегда нужного размера
        while (items.Count < maxSlots)
            items.Add(null);
    }

    // Добавление предмета в слот
    public bool AddItem(Item newItem, int slotIndex)
    {
        if (newItem == null)
        {
            Debug.LogWarning("Попытка добавить null в Inventory!");
            return false;
        }

        if (slotIndex < 0 || slotIndex >= maxSlots)
        {
            Debug.LogWarning("Неверный индекс слота для AddItem: " + slotIndex);
            return false;
        }

        if (items[slotIndex] != null)
        {
            Debug.Log("Слот " + slotIndex + " занят. Старый предмет остаётся.");
            return false;
        }

        newItem.gameObject.SetActive(false);
        items[slotIndex] = newItem;

        Rigidbody rb = newItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // выключаем физику пока предмет в инвентаре
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Поднял: " + newItem.itemName + " в слот " + slotIndex);
        return true;
    }

    // Выброс предмета перед игроком
    public bool DropItem(int slotIndex, Transform playerTransform)
    {
        if (slotIndex < 0 || slotIndex >= maxSlots)
            return false;

        Item item = items[slotIndex];
        if (item == null)
        {
            Debug.Log("В слоте " + slotIndex + " ничего нет.");
            return false;
        }

        items[slotIndex] = null;
        item.gameObject.SetActive(true);

        Vector3 dropPos = playerTransform.position + playerTransform.forward * dropForwardDistance + Vector3.up * dropHeight;

        // Raycast вниз для пола
        RaycastHit hit;
        int groundLayer = LayerMask.GetMask("Ground");
        if (Physics.Raycast(dropPos, Vector3.down, out hit, 5f, groundLayer))
            dropPos.y = hit.point.y + 0.1f;

        item.transform.position = dropPos;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // включаем физику
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce((playerTransform.forward + Vector3.up * 0.5f) * 2f, ForceMode.Impulse);
        }

        Debug.Log("Выбросил: " + item.itemName + " из слота " + slotIndex);

        // Проверяем крафт вокруг дропнутого предмета
        CraftManager cm = Object.FindFirstObjectByType<CraftManager>();
        if (cm != null)
            cm.CheckCraft(dropPos);
        else
            Debug.LogWarning("CraftManager не найден на сцене!");

        return true;
    }
}
