using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    public WorldConfig config;

    private List<WorldObject> worldObjects = new List<WorldObject>();
    private List<GeneticAgent> agents = new List<GeneticAgent>();

    private float evolutionTimer = 0f;

    private void Awake()
    {
        Instance = this;
        if (config == null) config = Resources.Load<WorldConfig>("WorldConfig");
    }

    private void Start()
    {
        ProceduralTerrain.GenerateTerrain();
        ResourceSpawner.SpawnInitialResources(config.InitialResources);
        SpawnInitialAgents(config.InitialAgents);
        ExternalWorldAdapter.Instance.RegisterAllObjects();
    }


    private void SpawnInitialAgents(int count)
    {
        // TODO: Добавьте сюда логику спавна агентов. Например:
        // for (int i = 0; i < count; i++) { Instantiate(agentPrefab, ...); }
        Debug.Log($"Нужно заспавнить {count} агентов, но метод пока пуст!");
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

    public void RegisterObject(WorldObject obj)
    {
        worldObjects.Add(obj);
        if (obj is GeneticAgent agent) agents.Add(agent);
    }

    public void UnregisterObject(WorldObject obj)
    {
        worldObjects.Remove(obj);
        if (obj is GeneticAgent agent) agents.Remove(agent);
    }

    public List<GeneticAgent> GetAgents() => agents;
    public List<WorldObject> GetWorldObjects() => worldObjects;
}