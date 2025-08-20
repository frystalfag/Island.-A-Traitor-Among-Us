using UnityEngine;
using TMPro;

public class PlayerPickScript : MonoBehaviour
{
    public float pickRange = 3f;
    public KeyCode slot1Key = KeyCode.Q;
    public KeyCode slot2Key = KeyCode.E;

    public Inventory inventory;
    public Camera playerCamera;
    public TMP_Text pickupHint;
    public CanvasGroup hintCanvasGroup;
    public float fadeSpeed = 5f;

    private Item currentTarget = null;

    void Update()
    {
        if (playerCamera == null || inventory == null || pickupHint == null || hintCanvasGroup == null)
            return;

        CheckTarget();
        HandleInput();

        // Плавное появление/исчезновение подсказки
        hintCanvasGroup.alpha = Mathf.MoveTowards(hintCanvasGroup.alpha, currentTarget != null ? 1f : 0f, fadeSpeed * Time.deltaTime);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(slot1Key)) HandleSlot(0);
        if (Input.GetKeyDown(slot2Key)) HandleSlot(1);
    }

    void HandleSlot(int slotIndex)
    {
        if (inventory.items[slotIndex] != null)
        {
            // Если слот занят — выбрасываем
            inventory.DropItem(slotIndex);
        }
        else if (currentTarget != null)
        {
            // Если слот пуст — подбираем
            inventory.AddItem(currentTarget, slotIndex);
            currentTarget = null;
        }
    }

    void CheckTarget()
    {
        currentTarget = null;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, pickRange))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null) currentTarget = item;
        }

        // Подсказка [Q]/[E] и название предмета
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
}
