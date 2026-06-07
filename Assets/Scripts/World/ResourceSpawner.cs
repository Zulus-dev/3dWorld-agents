using UnityEngine;
using System.Collections;

public class ResourceSpawner : MonoBehaviour
{
    public GameObject energyCrystalPrefab;
    public GameObject foodPrefab;
    public GameObject blockPrefab;

    public static void SpawnInitialResources(int count)
{
    // Находим спавнер ОДИН раз перед циклом
    ResourceSpawner spawner = FindObjectOfType<ResourceSpawner>();
    if (spawner == null) return; 

    for (int i = 0; i < count; i++)
    {
        Vector3 pos = new Vector3(Random.Range(-200, 200), 100, Random.Range(-200, 200));
        pos.y = Terrain.activeTerrain.SampleHeight(pos) + 2f;

        int type = Random.Range(0, 3);
        // Используем сохраненную ссылку spawner вместо постоянного поиска
        GameObject prefab = type == 0 ? spawner.energyCrystalPrefab :
                          type == 1 ? spawner.foodPrefab :
                          spawner.blockPrefab;

        Instantiate(prefab, pos, Quaternion.identity);
    }
}

    private void Start()
    {
        StartCoroutine(RegenerateFood());
    }

    private IEnumerator RegenerateFood()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(300, 600));
            Vector3 pos = new Vector3(Random.Range(-200, 200), 100, Random.Range(-200, 200));
            pos.y = Terrain.activeTerrain.SampleHeight(pos) + 1f;
            Instantiate(foodPrefab, pos, Quaternion.identity);
        }
    }
}