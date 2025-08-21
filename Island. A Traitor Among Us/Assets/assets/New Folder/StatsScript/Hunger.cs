using UnityEngine;
using UnityEngine.UI; // Добавляем для работы со Slider

public class HungerSystem : MonoBehaviour
{
    public float maxHunger = 100f;
    public float currentHunger;
    public float hungerDecreaseRate = 1f;
    public Slider hungerSlider; // Ссылка на Slider голода

    void Start()
    {
        currentHunger = maxHunger;
        if (hungerSlider != null)
        {
            hungerSlider.maxValue = maxHunger;
            hungerSlider.value = currentHunger;
        }
    }

    void Update()
    {
        currentHunger -= hungerDecreaseRate * Time.deltaTime;
        if (hungerSlider != null)
        {
            hungerSlider.value = currentHunger;
        }
        if (currentHunger <= 0)
        {
            GetComponent<HealthSystem>().TakeDamage(1f * Time.deltaTime);
        }
    }

    public void Eat(float amount)
    {
        currentHunger += amount;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
        if (hungerSlider != null)
        {
            hungerSlider.value = currentHunger;
        }
    }
}