using UnityEngine;
using TMPro;

public class TemperatureSystem : MonoBehaviour
{
    public float normalTemperature = 36.6f;
    public float currentTemperature;
    public float minTemperature = 30f;
    public float maxTemperature = 42f;

    // Сделаем это публичным, чтобы другие скрипты могли его менять
    public float environmentTemperature; 
    public TMP_Text temperatureText;
    private HealthSystem healthSystem;

    void Start()
    {
        currentTemperature = normalTemperature;
        healthSystem = GetComponent<HealthSystem>();
        UpdateUI();
    }

    void Update()
    {
        if (currentTemperature > environmentTemperature)
        {
            currentTemperature -= 0.5f * Time.deltaTime;
        }
        else if (currentTemperature < environmentTemperature)
        {
            currentTemperature += 0.5f * Time.deltaTime;
        }

        if (currentTemperature <= minTemperature || currentTemperature >= maxTemperature)
        {
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(2f * Time.deltaTime);
            }
        }
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (temperatureText != null)
        {
            temperatureText.text = currentTemperature.ToString("F1") + "°C";
        }
    }
}