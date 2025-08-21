using UnityEngine;
using System.Collections;

public class PlayerStatsUpdater : MonoBehaviour
{
    private HealthSystem healthSystem;
    private HungerSystem hungerSystem;
    private TemperatureSystem temperatureSystem;

    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        hungerSystem = GetComponent<HungerSystem>();
        temperatureSystem = GetComponent<TemperatureSystem>();

        // Запускаем корутину для обновления UI каждую секунду
        StartCoroutine(UpdateStatsUI());
    }

    IEnumerator UpdateStatsUI()
    {
        while (true)
        {
            if (healthSystem != null)
            {
                healthSystem.UpdateUI();
            }
            if (hungerSystem != null)
            {
                hungerSystem.UpdateUI();
            }
            if (temperatureSystem != null)
            {
                temperatureSystem.UpdateUI();
            }
            
            // Ждем одну секунду перед следующим обновлением
            yield return new WaitForSeconds(1f);
        }
    }
}