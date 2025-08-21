using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class PlayerPickScript : MonoBehaviour
{
    [Header("Interaction")]
    public float pickRange = 3f;
    public KeyCode pickKeyE = KeyCode.E;
    public KeyCode pickKeyQ = KeyCode.Q;
    public KeyCode closeKeyX = KeyCode.X;

    [Header("Inventory & Chests")]
    public Inventory inventory;

    [Header("Camera & UI")]
    public Camera playerCamera;
    public TMP_Text pickupHint;
    public CanvasGroup hintCanvasGroup;
    public float fadeSpeed = 5f;

    [Header("Chest Interaction")]
    public CanvasGroup chestUIPanel;
    public ChestUI chestUI;
    private Chest currentChest = null;
    private Item currentTarget = null;
    
    private HungerSystem hungerSystem;
    private HealthSystem healthSystem;
    private TemperatureSystem temperatureSystem;

    private bool isChestUIOpen = false;

    void Start()
    {
        hungerSystem = GetComponent<HungerSystem>();
        healthSystem = GetComponent<HealthSystem>();
        temperatureSystem = GetComponent<TemperatureSystem>();
    }

    void Update()
    {
        if (playerCamera == null || inventory == null || pickupHint == null || hintCanvasGroup == null)
            return;

        if (isChestUIOpen)
        {
            HandleChestUIInteraction();
        }
        else
        {
            HandleWorldInteraction();
        }

        UpdateUI();
    }

    void HandleWorldInteraction()
    {
        currentTarget = null;
        currentChest = null;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickRange))
        {
            Chest chest = hit.collider.GetComponentInParent<Chest>();
            if (chest != null)
            {
                currentChest = chest;
                pickupHint.text = $"Нажмите [{pickKeyE}] чтобы открыть сундук";
            }
            else
            {
                Item item = hit.collider.GetComponent<Item>();
                if (item != null)
                {
                    currentTarget = item;
                    UpdateItemHint();
                }
            }
        }
        else
        {
            pickupHint.text = "";
        }

        if (Input.GetKeyDown(pickKeyQ))
        {
            if (inventory.items[0] is ConsumableItem)
            {
                UseItemFromSlotQ();
            }
            else if (currentTarget != null)
            {
                inventory.AddItem(currentTarget, 0);
                currentTarget = null;
            }
            else if (inventory.items[0] != null)
            {
                inventory.DropItem(0, transform);
            }
        }

        if (Input.GetKeyDown(pickKeyE))
        {
            if (currentChest != null)
            {
                isChestUIOpen = true;
                if (chestUI != null)
                {
                    chestUI.chest = currentChest;
                    chestUI.UpdateUI();
                }
            }
            else if (inventory.items[1] is ConsumableItem)
            {
                UseItemFromSlotE();
            }
            else if (currentTarget != null)
            {
                inventory.AddItem(currentTarget, 1);
                currentTarget = null;
            }
            else if (inventory.items[1] != null)
            {
                inventory.DropItem(1, transform);
            }
        }
    }

    void HandleChestUIInteraction()
    {
        if (Input.GetKeyDown(closeKeyX))
        {
            isChestUIOpen = false;
            return;
        }

        if (Input.GetKeyDown(pickKeyE))
        {
            bool wasItemAdded = false;
            if (inventory.items[0] != null && currentChest != null)
            {
                Item itemToStore = inventory.items[0];
                if (currentChest.TryStoreItem(itemToStore))
                {
                    inventory.items[0] = null;
                    wasItemAdded = true;
                }
            }
            if (inventory.items[1] != null && currentChest != null)
            {
                Item itemToStore = inventory.items[1];
                if (currentChest.TryStoreItem(itemToStore))
                {
                    inventory.items[1] = null;
                    wasItemAdded = true;
                }
            }
            if (wasItemAdded)
            {
                chestUI.UpdateUI();
            }
        }
        
        if (Input.GetKeyDown(pickKeyQ))
        {
            bool wasItemTaken = false;
            for (int i = 0; i < inventory.items.Count; i++)
            {
                if (inventory.items[i] == null)
                {
                    Item itemToTake = currentChest.items.FirstOrDefault(item => item != null);
                    if (itemToTake != null)
                    {
                        currentChest.RemoveItem(currentChest.items.IndexOf(itemToTake));
                        inventory.AddItem(itemToTake, i);
                        wasItemTaken = true;
                    }
                }
            }
            if (wasItemTaken)
            {
                chestUI.UpdateUI();
            }
        }
    }

    void UseItemFromSlotQ()
    {
        Item itemToUse = inventory.items[0];
        if (itemToUse != null)
        {
            ConsumableItem consumableItem = itemToUse as ConsumableItem;
            if (consumableItem != null)
            {
                consumableItem.Use(gameObject);
                inventory.items[0] = null;
                // Обновляем UI после использования
                hungerSystem?.UpdateUI();
                healthSystem?.UpdateUI();
                temperatureSystem?.UpdateUI();
            }
        }
    }

    void UseItemFromSlotE()
    {
        Item itemToUse = inventory.items[1];
        if (itemToUse != null)
        {
            ConsumableItem consumableItem = itemToUse as ConsumableItem;
            if (consumableItem != null)
            {
                consumableItem.Use(gameObject);
                inventory.items[1] = null;
                // Обновляем UI после использования
                hungerSystem?.UpdateUI();
                healthSystem?.UpdateUI();
                temperatureSystem?.UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        float hintTargetAlpha = (currentTarget != null || currentChest != null) ? 1f : 0f;
        hintCanvasGroup.alpha = Mathf.MoveTowards(hintCanvasGroup.alpha, hintTargetAlpha, fadeSpeed * Time.deltaTime);

        float chestTargetAlpha = isChestUIOpen ? 1f : 0f;
        chestUIPanel.alpha = Mathf.MoveTowards(chestUIPanel.alpha, chestTargetAlpha, fadeSpeed * Time.deltaTime);
        chestUIPanel.interactable = isChestUIOpen;
        chestUIPanel.blocksRaycasts = isChestUIOpen;

        if (isChestUIOpen)
        {
            pickupHint.text = $"Нажмите [{pickKeyE}] чтобы положить | [{pickKeyQ}] чтобы забрать | [{closeKeyX}] чтобы закрыть";
        }
    }

    void UpdateItemHint()
    {
        if (currentTarget != null)
        {
            string hint = "";
            if (inventory.items[0] == null) hint += $"[Q] ";
            if (inventory.items[1] == null) hint += $"[E] ";
            hint += $"Подобрать {currentTarget.itemName}";
            pickupHint.text = hint;
        }
        else
        {
            pickupHint.text = "";
        }
    }
}