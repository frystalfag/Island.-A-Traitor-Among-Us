using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Список UI-слотов для сундука. Перетащи сюда все слоты вручную.")]
    public List<Image> slots;
    [Tooltip("Ссылка на скрипт Chest.")]
    public Chest chest;

    void Start()
    {
        if (slots == null || slots.Count == 0)
        {
            Debug.LogError("Список слотов UI сундука пуст или не подключен в инспекторе!");
            return;
        }
        
        if (chest == null)
        {
            Debug.LogError("Скрипт Chest не подключен в инспекторе ChestUI!");
            return;
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slots == null || chest == null) return;
        
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < chest.items.Count && chest.items[i] != null)
            {
                // Отображаем иконку предмета, если он есть
                slots[i].enabled = true;
                slots[i].sprite = chest.items[i].itemIcon;
            }
            else
            {
                // Иначе скрываем слот
                slots[i].enabled = false;
            }
        }
    }
}