using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    public Chest chest;
    // Удаляем старые публичные переменные
    // public GameObject slotPrefab;
    // public Transform slotParent;

    // Новый публичный список для слотов, которые ты назначишь вручную
    public List<Image> slotImages = new List<Image>();

    private void Awake()
    {
        // Теперь Awake() ничего не делает, так как слоты уже существуют в сцене
        // и будут назначены через инспектор
    }

    // Этот метод теперь публичный
    public void UpdateUI()
    {
        if (chest == null || slotImages.Count == 0) return;

        for (int i = 0; i < slotImages.Count; i++)
        {
            if (i >= chest.storedItems.Count) continue;

            Item item = chest.storedItems[i];
            if (item != null)
            {
                slotImages[i].sprite = item.itemIcon;
                slotImages[i].color = Color.white;
            }
            else
            {
                slotImages[i].sprite = null;
                slotImages[i].color = new Color(1, 1, 1, 0);
            }
        }
    }
}