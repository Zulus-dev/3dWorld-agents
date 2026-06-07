using UnityEngine;

[CreateAssetMenu(fileName = "WorldConfig", menuName = "Ecosystem/WorldConfig")]
public class WorldConfig : ScriptableObject
{
    [Header("World Settings")]
    public Vector3 WorldSize = new Vector3(500, 300, 500);
    public int InitialAgents = 20;
    public int TargetPopulation = 80;
    public int MinimumPopulation = 20;
    public int InitialResources = 150;
    public float EvolutionInterval = 1200f;
    public float SnapshotInterval = 1f;
    public int RandomSeed = 0;
    public bool LogEventsToConsole = false;

    [Header("Prefabs")]
    public GameObject AgentPrefab;
    public GameObject EnergyCrystalPrefab;
    public GameObject FoodPrefab;
    public GameObject BlockPrefab;
    public GameObject CorpseResourcePrefab;

    [Header("Terrain")]
    public int TerrainResolution = 513;
    public float TerrainMaxHeight = 90f;
    public float BaseNoiseFrequency = 0.006f;
    public int NoiseOctaves = 4;
    public float TerrainHeightScale = 0.55f;
    public int TerrainSmoothIterations = 3;
    public float SpawnHeightOffset = 3f;

    [Header("Genetic Algorithm")]
    public float MutationRate = 0.12f;
    public float MutationStrength = 0.25f;
    public int TournamentSize = 5;
    public float EliteRatio = 0.1f;

    [Header("Fitness Weights")]
    public float LifetimeWeight = 0.4f;
    public float EnergyWeight = 0.25f;
    public float StructuresWeight = 0.2f;
    public float ExplorationWeight = 0.1f;
    public float OffspringWeight = 0.05f;

    [Header("Agent Settings")]
    public float BaseMetabolism = 0.15f;
    public float EnergyDeathThreshold = 0f;
    public float ReproductionEnergyThreshold = 120f;
    public float ReproductionCost = 55f;
    public float MaxAgentAge = 1800f;
    public float ResourcePickupDistance = 2f;

    [Header("Resources")]
    public int TargetFoodCount = 120;
    public int TargetEnergyCrystalCount = 80;
    public int TargetLooseBlockCount = 120;
    public float ResourceRegenCheckInterval = 10f;
    public int ResourceRegenBatchSize = 12;

    [Header("Building")]
    public float BuildEnergyCost = 15f;
    public float BuildCooldown = 3f;
    public float BuildDistance = 2.5f;
    public float StabilityCheckRadius = 0.75f;
}
