using UnityEngine;

public class GrassSpawnerMulti : MonoBehaviour
{
    [Header("Префабы растений (трава, папоротник, цветы и т.д.)")]
    public GameObject[] plantPrefabs;   // Сюда закидываешь несколько префабов

    [Header("Настройки спавна")]
    public int plantCount = 200;        // Сколько всего сгенерить
    public float areaSize = 5f;         // Размер области вокруг объекта
    public float minScale = 0.8f;       // Минимальный размер
    public float maxScale = 1.3f;       // Максимальный размер

    void Start()
    {
        for (int i = 0; i < plantCount; i++)
        {
            // Рандомная точка в пределах areaSize
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-areaSize, areaSize),
                10f, // сверху
                Random.Range(-areaSize, areaSize)
            );

            // Луч вниз
            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 50f))
            {
                if (hit.collider.gameObject == gameObject) // Только по этому объекту
                {
                    // Берём случайный префаб из списка
                    GameObject prefab = plantPrefabs[Random.Range(0, plantPrefabs.Length)];
                    
                    // Создаём объект
                    GameObject g = Instantiate(prefab, hit.point, Quaternion.identity);

                    // ❌ Больше не выравниваем по нормали
                    // g.transform.up = hit.normal;

                    // ✅ Всегда строго вверх
                    g.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

                    // Случайный масштаб
                    float scale = Random.Range(minScale, maxScale);
                    g.transform.localScale *= scale;

                    // Делаем дочерним объектом
                    g.transform.SetParent(transform);
                }
            }
        }
    }
}
