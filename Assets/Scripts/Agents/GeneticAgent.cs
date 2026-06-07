using UnityEngine;

[RequireComponent(typeof(GeneticController))]
[RequireComponent(typeof(AgentPhysics))]
[RequireComponent(typeof(AgentSensors))]
[RequireComponent(typeof(AgentBrain))]
[RequireComponent(typeof(AgentEnergySystem))]
[RequireComponent(typeof(AgentBuilder))]
public class GeneticAgent : WorldObject
{
    public GeneticController geneticController;
    public AgentPhysics physics;
    public AgentSensors sensors;
    public AgentBrain brain;
    public AgentEnergySystem energySystem;
    public AgentBuilder builder;

    public Genotype PendingGenotype { get; set; }
    public bool PendingMutation { get; set; }
    public int StructuresBuiltCount { get; private set; }
    public int NewChunksExplored { get; private set; }
    public int OffspringCount { get; private set; }

    private Vector2Int lastExplorationCell = new Vector2Int(int.MinValue, int.MinValue);

    protected override void Awake()
    {
        base.Awake();
        data.type = "Agent";
    }

    private void Start()
    {
        CacheComponents();
        geneticController.agent = this;
        geneticController.Initialize(PendingGenotype, PendingMutation);
        ApplyGenotype();
        WorldEventLogger.LogEvent("Birth", objectId, string.Empty, transform.position);
    }

    private void FixedUpdate()
    {
        if (geneticController == null || geneticController.genotype == null) return;

        sensors.UpdateSensors();
        Vector3 desiredMove = brain.ProcessInputs();
        physics.ApplyMovement(desiredMove);

        if (brain.ShouldBuild() && energySystem.energy > WorldManager.Instance.config.BuildEnergyCost)
            builder.AttemptBuild();

        if (energySystem.CanReproduce()) AttemptReproduce();
        TrackExploration();
    }

    public void NotifyStructureBuilt()
    {
        StructuresBuiltCount++;
    }

    public void Die()
    {
        WorldEventLogger.LogEvent("Death", objectId, string.Empty, transform.position);
        Destroy(gameObject, 0.1f);
    }

    private void AttemptReproduce()
    {
        WorldConfig config = WorldManager.Instance.config;
        Vector3 spawnPos = transform.position + Random.insideUnitSphere * 3f;
        spawnPos.y = WorldManager.Instance.SampleTerrainHeight(spawnPos) + config.SpawnHeightOffset;

        GeneticAgent child = WorldManager.Instance.SpawnAgent(geneticController.genotype, spawnPos, true);
        if (child == null) return;

        energySystem.energy -= config.ReproductionCost;
        OffspringCount++;
        WorldEventLogger.LogEvent("Reproduction", objectId, child.objectId, spawnPos);
    }

    private void CacheComponents()
    {
        geneticController = GetComponent<GeneticController>();
        physics = GetComponent<AgentPhysics>();
        sensors = GetComponent<AgentSensors>();
        brain = GetComponent<AgentBrain>();
        energySystem = GetComponent<AgentEnergySystem>();
        builder = GetComponent<AgentBuilder>();
    }

    private void ApplyGenotype()
    {
        physics.ApplyGenotype(geneticController.genotype);
        sensors.ApplyGenotype(geneticController.genotype);
        energySystem.ApplyGenotype(geneticController.genotype);

        AgentBody body = GetComponent<AgentBody>();
        if (body != null) body.ApplyGenotypeVisuals();
    }

    private void TrackExploration()
    {
        const float cellSize = 50f;
        Vector2Int cell = new Vector2Int(Mathf.FloorToInt(transform.position.x / cellSize), Mathf.FloorToInt(transform.position.z / cellSize));
        if (cell != lastExplorationCell)
        {
            lastExplorationCell = cell;
            NewChunksExplored++;
        }
    }
}
