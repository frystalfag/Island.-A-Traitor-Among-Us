using UnityEngine;

public class Campfire : MonoBehaviour
{
    public float range = 5f;
    public float temperatureIncrease = 10f;

    private Transform player;
    private TemperatureSystem temperatureSystem;

    void Start()
    {
        player = FindObjectOfType<PlayerPickScript>().transform;
        temperatureSystem = FindObjectOfType<TemperatureSystem>();
    }

    void Update()
    {
        if (player == null || temperatureSystem == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= range)
        {
            // Увеличиваем температуру окружающей среды для игрока
            temperatureSystem.environmentTemperature += temperatureIncrease;
        }
    }

    // Это визуализация радиуса костра в редакторе Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}