using UnityEngine;
using UnityEngine.UI;

public class HungerSystem : MonoBehaviour
{
    public float maxHunger = 100f;
    public float currentHunger;
    public float hungerDecreaseRate = 1f;
    public Slider hungerSlider;
    private HealthSystem healthSystem;

    void Start()
    {
        currentHunger = maxHunger;
        healthSystem = GetComponent<HealthSystem>();
        UpdateUI();
    }

    void Update()
    {
        currentHunger -= hungerDecreaseRate * Time.deltaTime;
        if (currentHunger <= 0)
        {
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(1f * Time.deltaTime);
            }
        }
    }

    public void Eat(float amount)
    {
        currentHunger += amount;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
    }
    
    // Метод, который будет вызываться из Update() другого скрипта
    public void UpdateUI()
    {
        if (hungerSlider != null)
        {
            hungerSlider.maxValue = maxHunger;
            hungerSlider.value = currentHunger;
        }
    }
}