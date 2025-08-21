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
    
    // Private references to other systems
    private HungerSystem hungerSystem;
    private HealthSystem healthSystem;
    private TemperatureSystem temperatureSystem;

    private bool isChestUIOpen = false;

    void Start()
    {
        Debug.Log("PlayerPickScript: Start method called.");
        // Attempt to get references to the system scripts
        hungerSystem = GetComponent<HungerSystem>();
        healthSystem = GetComponent<HealthSystem>();
        temperatureSystem = GetComponent<TemperatureSystem>();

        // Check if the references were successfully found
        if (hungerSystem == null)
        {
            Debug.LogError("Error: HungerSystem component not found on this GameObject. Make sure it's attached.");
        }
        else
        {
            Debug.Log("Success: HungerSystem component found.");
        }

        if (healthSystem == null)
        {
            Debug.LogError("Error: HealthSystem component not found on this GameObject. Make sure it's attached.");
        }
        else
        {
            Debug.Log("Success: HealthSystem component found.");
        }

        if (temperatureSystem == null)
        {
            Debug.LogError("Error: TemperatureSystem component not found on this GameObject. Make sure it's attached.");
        }
        else
        {
            Debug.Log("Success: TemperatureSystem component found.");
        }
    }

    void Update()
    {
        // ... (existing code for handling world and chest interactions)
        if (playerCamera == null || inventory == null || pickupHint == null || hintCanvasGroup == null)
        {
            return;
        }

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
                UseItem(0);
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
                UseItem(1);
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

    void UseItem(int slotIndex)
    {
        Debug.Log($"Attempting to use item from slot: {slotIndex}");

        // Check if the item in the inventory is a ConsumableItem
        if (inventory == null || inventory.items == null || inventory.items.Count <= slotIndex || !(inventory.items[slotIndex] is ConsumableItem))
        {
            Debug.LogWarning($"Warning: Item in slot {slotIndex} is either null or not a ConsumableItem. Action cancelled.");
            return;
        }

        ConsumableItem consumableItem = inventory.items[slotIndex] as ConsumableItem;

        Debug.Log($"Using item: {consumableItem.itemName}.");

        // Use the item's effects and check if the systems exist
        if (hungerSystem != null)
        {
            hungerSystem.Eat(consumableItem.hungerRestoreAmount);
            Debug.Log($"Hunger stat updated. Current hunger: {hungerSystem.currentHunger}");
        }
        else
        {
            Debug.LogWarning("HungerSystem is null. Cannot update hunger.");
        }

        if (healthSystem != null)
        {
            healthSystem.Heal(consumableItem.healthRestoreAmount);
            Debug.Log($"Health stat updated. Current health: {healthSystem.currentHealth}");
        }
        else
        {
            Debug.LogWarning("HealthSystem is null. Cannot update health.");
        }

        if (temperatureSystem != null)
        {
            temperatureSystem.currentTemperature += consumableItem.temperatureChangeAmount;
            Debug.Log($"Temperature stat updated. Current temperature: {temperatureSystem.currentTemperature}");
        }
        else
        {
            Debug.LogWarning("TemperatureSystem is null. Cannot update temperature.");
        }
        
        // Finally, remove the item from the inventory
        inventory.items[slotIndex] = null;
        Debug.Log($"Item {consumableItem.itemName} successfully consumed and removed from inventory.");
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