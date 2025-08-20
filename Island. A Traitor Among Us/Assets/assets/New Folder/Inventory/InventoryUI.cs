using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Image[] slotImages;

    void UpdateUI()
    {
        if (inventory == null || slotImages == null)
            return;

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < inventory.items.Count && inventory.items[i] != null)
            {
                slotImages[i].sprite = inventory.items[i].itemIcon;
                slotImages[i].enabled = true;
            }
            else if (i < slotImages.Length)
            {
                slotImages[i].sprite = null;
                slotImages[i].enabled = false;
            }
        }
    }

    void Update()
    {
        UpdateUI();
    }
}
