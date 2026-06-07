using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    public WorldConfig config;

    private readonly List<WorldObject> worldObjects = new List<WorldObject>();
    private readonly List<GeneticAgent> agents = new List<GeneticAgent>();

    private float evolutionTimer;
    private float snapshotTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (config == null) config = Resources.Load<WorldConfig>("WorldConfig");
        if (config != null && config.RandomSeed != 0) Random.InitState(config.RandomSeed);
        EnsureAdapter();
    }

    private void Start()
    {
        if (config == null)
        {
            Debug.LogError("WorldConfig is missing. Create and assign Assets/ScriptableObjects/WorldConfig.asset.");
            enabled = false;
            return;
        }

        ProceduralTerrain.GenerateTerrain(config);
        ResourceSpawner.SpawnInitialResources(config.InitialResources);
        SpawnInitialAgents(config.InitialAgents);

        if (ExternalWorldAdapter.Instance != null)
            ExternalWorldAdapter.Instance.RegisterAllObjects();
    }

    private void FixedUpdate()
    {
        evolutionTimer += Time.fixedDeltaTime;
        if (evolutionTimer >= config.EvolutionInterval)
        {
            GeneticAlgorithm.Evolve();
            evolutionTimer = 0f;
        }
    }

    private void LateUpdate()
    {
        snapshotTimer += Time.deltaTime;
        if (snapshotTimer >= config.SnapshotInterval)
        {
            snapshotTimer = 0f;
            if (ExternalWorldAdapter.Instance != null)
                ExternalWorldAdapter.Instance.PushWorldSnapshot();
        }
    }

    private void SpawnInitialAgents(int count)
    {
        for (int i = 0; i < count; i++)
            SpawnAgent(Genotype.CreateRandom(), GetRandomSpawnPosition(), false);
    }

    public GeneticAgent SpawnAgent(Genotype genotype, Vector3 position, bool mutate)
    {
        GameObject prefab = ResolvePrefab(config.AgentPrefab, "Prefabs/GeneticAgent");
        if (prefab == null)
        {
            Debug.LogError("Agent prefab is not assigned and was not found in Resources/Prefabs/GeneticAgent.");
            return null;
        }

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        GeneticAgent agent = instance.GetComponent<GeneticAgent>();
        if (agent == null)
        {
            Debug.LogError("Agent prefab must contain GeneticAgent component.");
            Destroy(instance);
            return null;
        }

        agent.PendingGenotype = genotype;
        agent.PendingMutation = mutate;
        return agent;
    }

    public WorldObject SpawnBlock(Vector3 position, Quaternion rotation, string ownerAgentId)
    {
        GameObject prefab = ResolvePrefab(config.BlockPrefab, "Prefabs/Block");
        if (prefab == null) return null;

        GameObject instance = Instantiate(prefab, position, rotation);
        WorldObject obj = instance.GetComponent<WorldObject>();
        if (obj != null)
        {
            obj.data.type = "Structure";
            obj.data.ownerAgentId = ownerAgentId;
            obj.data.energy = 0f;
        }
        return obj;
    }

    public Vector3 GetRandomSpawnPosition()
    {
        Vector3 half = config.WorldSize * 0.5f;
        Vector3 best = Vector3.zero;

        for (int i = 0; i < 24; i++)
        {
            Vector3 candidate = new Vector3(Random.Range(-half.x, half.x), config.WorldSize.y, Random.Range(-half.z, half.z));
            candidate.y = SampleTerrainHeight(candidate) + config.SpawnHeightOffset;
            if (IsReasonablyFlat(candidate))
                return candidate;
            best = candidate;
        }

        return best;
    }

    public float SampleTerrainHeight(Vector3 position)
    {
        return Terrain.activeTerrain != null ? Terrain.activeTerrain.SampleHeight(position) : 0f;
    }

    private bool IsReasonablyFlat(Vector3 position)
    {
        float center = SampleTerrainHeight(position);
        float maxDelta = 0f;
        const float radius = 3f;

        maxDelta = Mathf.Max(maxDelta, Mathf.Abs(center - SampleTerrainHeight(position + Vector3.forward * radius)));
        maxDelta = Mathf.Max(maxDelta, Mathf.Abs(center - SampleTerrainHeight(position + Vector3.back * radius)));
        maxDelta = Mathf.Max(maxDelta, Mathf.Abs(center - SampleTerrainHeight(position + Vector3.left * radius)));
        maxDelta = Mathf.Max(maxDelta, Mathf.Abs(center - SampleTerrainHeight(position + Vector3.right * radius)));

        return maxDelta <= 2.5f;
    }

    public void RegisterObject(WorldObject obj)
    {
        if (obj == null || worldObjects.Contains(obj)) return;

        worldObjects.Add(obj);
        GeneticAgent agent = obj as GeneticAgent;
        if (agent != null && !agents.Contains(agent)) agents.Add(agent);

        if (ExternalWorldAdapter.Instance != null)
            ExternalWorldAdapter.Instance.RegisterObject(obj);
    }

    public void UnregisterObject(WorldObject obj)
    {
        worldObjects.Remove(obj);
        GeneticAgent agent = obj as GeneticAgent;
        if (agent != null) agents.Remove(agent);
    }

    public CompactWorldSnapshot CreateCompactSnapshot()
    {
        List<TriadicObservation> observations = new List<TriadicObservation>(worldObjects.Count);
        for (int i = 0; i < worldObjects.Count; i++)
        {
            WorldObject obj = worldObjects[i];
            if (obj == null) continue;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            float energy = obj.data != null ? obj.data.energy : 0f;
            AgentEnergySystem energySystem = obj.GetComponent<AgentEnergySystem>();
            if (energySystem != null) energy = energySystem.energy;

            observations.Add(new TriadicObservation
            {
                objectId = obj.objectId,
                state = obj.gameObject.activeInHierarchy ? 1 : 0,
                position = obj.transform.position,
                velocity = rb != null ? rb.velocity : Vector3.zero,
                energyLevel = energy,
                confidence = 1f,
                patternHash = obj.data != null ? obj.data.type.GetHashCode() : 0
            });
        }

        return new CompactWorldSnapshot
        {
            timestamp = Time.time,
            agentCount = agents.Count,
            objectCount = worldObjects.Count,
            observations = observations.ToArray()
        };
    }

    public List<GeneticAgent> GetAgents()
    {
        agents.RemoveAll(a => a == null);
        return agents;
    }

    public List<WorldObject> GetWorldObjects()
    {
        worldObjects.RemoveAll(o => o == null);
        return worldObjects;
    }

    private GameObject ResolvePrefab(GameObject assignedPrefab, string resourcesPath)
    {
        return assignedPrefab != null ? assignedPrefab : Resources.Load<GameObject>(resourcesPath);
    }

    private void EnsureAdapter()
    {
        if (ExternalWorldAdapter.Instance != null) return;
        GameObject adapter = new GameObject("ExternalWorldAdapter");
        adapter.AddComponent<ExternalWorldAdapter>();
    }
}
