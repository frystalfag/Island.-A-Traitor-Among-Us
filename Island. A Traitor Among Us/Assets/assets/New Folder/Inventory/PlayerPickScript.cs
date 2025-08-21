using UnityEngine;
using TMPro;

public class PlayerPickScript : MonoBehaviour
{
    [Header("Interaction")]
    public float pickRange = 3f;
    public KeyCode pickKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;
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

    private bool isChestUIOpen = false;

    void Update()
    {
        if (playerCamera == null || inventory == null || pickupHint == null || hintCanvasGroup == null)
            return;

        HandleRaycast();
        HandleInput();
        UpdateUI();
    }

    void HandleRaycast()
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
                pickupHint.text = "Нажмите [E] чтобы открыть сундук";
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

        float targetAlpha = (currentTarget != null || currentChest != null) ? 1f : 0f;
        hintCanvasGroup.alpha = Mathf.MoveTowards(hintCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }

    void UpdateItemHint()
    {
        if (currentTarget != null)
        {
            string hint = "";
            if (inventory.items[0] == null) hint += "[Q] ";
            if (inventory.items[1] == null) hint += "[E] ";
            hint += currentTarget.itemName;
            pickupHint.text = hint;
        }
        else
        {
            pickupHint.text = "";
        }
    }

    void HandleInput()
    {
        if (currentChest != null && Input.GetKeyDown(pickKey))
        {
            isChestUIOpen = !isChestUIOpen;
            if (isChestUIOpen)
            {
                if (chestUI != null)
                {
                    chestUI.chest = currentChest;
                    chestUI.UpdateUI();
                }
            }
        }

        if (!isChestUIOpen)
        {
            if (Input.GetKeyDown(dropKey))
            {
                if (inventory.items[0] != null)
                {
                    // Вызываем DropItem с позицией игрока
                    inventory.DropItem(0, transform);
                }
                else if (currentTarget != null)
                {
                    if (currentChest != null)
                    {
                        currentChest.TryStoreItem(currentTarget);
                    }
                    else
                    {
                        inventory.AddItem(currentTarget, 0);
                    }
                    currentTarget = null;
                }
            }
            if (Input.GetKeyDown(pickKey))
            {
                if (inventory.items[1] != null)
                {
                    // Вызываем DropItem с позицией игрока
                    inventory.DropItem(1, transform);
                }
                else if (currentTarget != null)
                {
                    if (currentChest != null)
                    {
                        currentChest.TryStoreItem(currentTarget);
                    }
                    else
                    {
                        inventory.AddItem(currentTarget, 1);
                    }
                    currentTarget = null;
                }
            }
        }
    }

    void UpdateUI()
    {
        if (chestUIPanel != null)
        {
            float chestAlpha = isChestUIOpen ? 1f : 0f;
            chestUIPanel.alpha = Mathf.MoveTowards(chestUIPanel.alpha, chestAlpha, fadeSpeed * Time.deltaTime);
            chestUIPanel.interactable = isChestUIOpen;
            chestUIPanel.blocksRaycasts = isChestUIOpen;
        }
    }
}