using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Inventory/Consumable Item")]
public class ConsumableItem : Item
{
    public float hungerRestoreAmount = 20f;
    public float healthRestoreAmount = 0f;
    public float temperatureChangeAmount = 0f;

    public void Use(GameObject player)
    {
        HungerSystem hungerSystem = player.GetComponent<HungerSystem>();
        if (hungerSystem != null)
        {
            hungerSystem.Eat(hungerRestoreAmount);
        }

        HealthSystem healthSystem = player.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.Heal(healthRestoreAmount);
        }

        TemperatureSystem temperatureSystem = player.GetComponent<TemperatureSystem>();
        if (temperatureSystem != null)
        {
            temperatureSystem.currentTemperature += temperatureChangeAmount;
        }

        Debug.Log("Предмет " + itemName + " был использован.");
    }
}