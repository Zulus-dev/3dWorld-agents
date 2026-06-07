using UnityEngine;
using System.Collections;

public class ResourceSpawner : MonoBehaviour
{
    public GameObject energyCrystalPrefab;
    public GameObject foodPrefab;
    public GameObject blockPrefab;

    public static void SpawnInitialResources(int count)
    {
        ResourceSpawner spawner = FindObjectOfType<ResourceSpawner>();
        WorldConfig config = WorldManager.Instance != null ? WorldManager.Instance.config : null;

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = ResolveResourcePrefab(spawner, config, Random.Range(0, 3));
            if (prefab == null) continue;
            Instantiate(prefab, RandomResourcePosition(config), Quaternion.identity);
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
            WorldConfig config = WorldManager.Instance != null ? WorldManager.Instance.config : null;
            GameObject prefab = foodPrefab != null ? foodPrefab : (config != null ? config.FoodPrefab : null);
            if (prefab != null)
                Instantiate(prefab, RandomResourcePosition(config), Quaternion.identity);
        }
    }

    private static GameObject ResolveResourcePrefab(ResourceSpawner spawner, WorldConfig config, int type)
    {
        if (type == 0)
            return FirstPrefab(spawner != null ? spawner.energyCrystalPrefab : null, config != null ? config.EnergyCrystalPrefab : null, "Prefabs/EnergyCrystal");
        if (type == 1)
            return FirstPrefab(spawner != null ? spawner.foodPrefab : null, config != null ? config.FoodPrefab : null, "Prefabs/Food");
        return FirstPrefab(spawner != null ? spawner.blockPrefab : null, config != null ? config.BlockPrefab : null, "Prefabs/Block");
    }

    private static GameObject FirstPrefab(GameObject scenePrefab, GameObject configPrefab, string resourcesPath)
    {
        if (scenePrefab != null) return scenePrefab;
        if (configPrefab != null) return configPrefab;
        return Resources.Load<GameObject>(resourcesPath);
    }

    private static Vector3 RandomResourcePosition(WorldConfig config)
    {
        Vector3 worldSize = config != null ? config.WorldSize : new Vector3(500, 300, 500);
        Vector3 half = worldSize * 0.5f;
        Vector3 pos = new Vector3(Random.Range(-half.x, half.x), worldSize.y, Random.Range(-half.z, half.z));
        pos.y = Terrain.activeTerrain != null ? Terrain.activeTerrain.SampleHeight(pos) + 2f : 2f;
        return pos;
    }
}
