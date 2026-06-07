using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceSpawner : MonoBehaviour
{
    public GameObject energyCrystalPrefab;
    public GameObject foodPrefab;
    public GameObject blockPrefab;

    public static void SpawnInitialResources(int count)
    {
        ResourceSpawner spawner = FindObjectOfType<ResourceSpawner>();
        WorldConfig config = WorldManager.Instance != null ? WorldManager.Instance.config : null;

        int food = Mathf.RoundToInt(count * 0.4f);
        int crystals = Mathf.RoundToInt(count * 0.3f);
        int blocks = Mathf.Max(0, count - food - crystals);

        SpawnBatch(spawner, config, "Food", food);
        SpawnBatch(spawner, config, "EnergyCrystal", crystals);
        SpawnBatch(spawner, config, "Block", blocks);
    }

    private void Start()
    {
        StartCoroutine(RegenerateResources());
    }

    private IEnumerator RegenerateResources()
    {
        while (true)
        {
            WorldConfig config = WorldManager.Instance != null ? WorldManager.Instance.config : null;
            float interval = config != null ? Mathf.Max(1f, config.ResourceRegenCheckInterval) : 10f;
            yield return new WaitForSeconds(interval);
            RefillResources(config);
        }
    }

    private void RefillResources(WorldConfig config)
    {
        if (WorldManager.Instance == null || config == null) return;

        int foodCount = CountWorldObjects("Food");
        int crystalCount = CountWorldObjects("EnergyCrystal");
        int looseBlockCount = CountWorldObjects("Block");
        int batchSize = Mathf.Max(1, config.ResourceRegenBatchSize);

        SpawnMissing(config, "Food", config.TargetFoodCount - foodCount, batchSize);
        SpawnMissing(config, "EnergyCrystal", config.TargetEnergyCrystalCount - crystalCount, batchSize);
        SpawnMissing(config, "Block", config.TargetLooseBlockCount - looseBlockCount, batchSize);
    }

    private void SpawnMissing(WorldConfig config, string type, int missing, int batchSize)
    {
        if (missing <= 0) return;
        SpawnBatch(this, config, type, Mathf.Min(missing, batchSize));
    }

    private static void SpawnBatch(ResourceSpawner spawner, WorldConfig config, string type, int count)
    {
        GameObject prefab = ResolveResourcePrefab(spawner, config, type);
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
            Instantiate(prefab, RandomResourcePosition(config), Quaternion.identity);
    }

    private static int CountWorldObjects(string type)
    {
        List<WorldObject> objects = WorldManager.Instance.GetWorldObjects();
        int count = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            WorldObject obj = objects[i];
            if (obj != null && obj.data != null && obj.data.type == type)
                count++;
        }
        return count;
    }

    private static GameObject ResolveResourcePrefab(ResourceSpawner spawner, WorldConfig config, string type)
    {
        if (type == "EnergyCrystal")
            return FirstPrefab(spawner != null ? spawner.energyCrystalPrefab : null, config != null ? config.EnergyCrystalPrefab : null, "Prefabs/EnergyCrystal");
        if (type == "Food")
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
        if (WorldManager.Instance != null)
        {
            Vector3 pos = WorldManager.Instance.GetRandomSpawnPosition();
            pos.y = WorldManager.Instance.SampleTerrainHeight(pos) + 2f;
            return pos;
        }

        Vector3 worldSize = config != null ? config.WorldSize : new Vector3(500, 300, 500);
        Vector3 half = worldSize * 0.5f;
        Vector3 fallback = new Vector3(Random.Range(-half.x, half.x), worldSize.y, Random.Range(-half.z, half.z));
        fallback.y = Terrain.activeTerrain != null ? Terrain.activeTerrain.SampleHeight(fallback) + 2f : 2f;
        return fallback;
    }
}
