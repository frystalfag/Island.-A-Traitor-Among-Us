using UnityEngine;
using TMPro;

public class PlayerPickScript : MonoBehaviour
{
    public float pickRange = 3f;
    public KeyCode pickKey = KeyCode.E; // слот 2
    public KeyCode dropKey = KeyCode.Q; // слот 1
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

        float targetAlpha = currentTarget != null ? 1f : 0f;
        hintCanvasGroup.alpha = Mathf.MoveTowards(hintCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }

    void HandleInput()
    {
        // Слот 1 (Q)
        if (Input.GetKeyDown(dropKey))
        {
            if (inventory.items[0] != null)
                inventory.DropItem(0, transform);
            else if (currentTarget != null)
            {
                inventory.AddItem(currentTarget, 0);
                currentTarget = null;
            }
        }

        // Слот 2 (E)
        if (Input.GetKeyDown(pickKey))
        {
            if (inventory.items[1] != null)
                inventory.DropItem(1, transform);
            else if (currentTarget != null)
            {
                inventory.AddItem(currentTarget, 1);
                currentTarget = null;
            }
        }
    }

    void CheckTarget()
    {
        currentTarget = null;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, pickRange))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
                currentTarget = item;
        }

        if (currentTarget != null)
        {
            string hint = "";
            if (inventory.items[0] == null) hint += "[Q] ";
            if (inventory.items[1] == null) hint += "[E] ";
            hint += currentTarget.itemName;
            pickupHint.text = hint;
        }
        else
            pickupHint.text = "";
    }
}
