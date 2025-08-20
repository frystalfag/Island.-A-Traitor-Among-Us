using UnityEngine;

public class ResourceSpawnerMulti : MonoBehaviour
{
    [Header("Префабы ресурсов (камни, деревья и т.д.)")]
    public GameObject[] resourcePrefabs;   // Сюда закидываешь несколько префабов

    [Header("Настройки спавна")]
    public int resourceCount = 200;        // Сколько всего сгенерить
    public float areaSize = 5f;            // Размер области вокруг объекта
    public float minScale = 0.8f;          // Минимальный размер
    public float maxScale = 1.3f;          // Максимальный размер

    void Start()
    {
        for (int i = 0; i < resourceCount; i++)
        {
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-areaSize, areaSize),
                10f,
                Random.Range(-areaSize, areaSize)
            );

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 50f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    GameObject prefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Length)];

                    GameObject g = Instantiate(prefab, hit.point, Quaternion.identity);

                    g.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

                    float scale = Random.Range(minScale, maxScale);
                    g.transform.localScale *= scale;

                    g.transform.SetParent(transform);
                }
            }
        }
    }
}
