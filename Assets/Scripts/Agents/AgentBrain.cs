using UnityEngine;
using System.Collections.Generic;

public class AgentBrain : MonoBehaviour
{
    public GeneticController geneticController;
    public AgentSensors sensors;
    public AgentIntention currentIntention;

    private AgentEnergySystem energySystem;
    private readonly Queue<WorldEvent> shortTermMemory = new Queue<WorldEvent>();
    private const int MemoryLimit = 10;

    private void Awake()
    {
        CacheReferences();
    }

    private void Reset()
    {
        CacheReferences();
    }

    private void OnValidate()
    {
        CacheReferences();
    }

    private void CacheReferences()
    {
        if (geneticController == null) geneticController = GetComponent<GeneticController>();
        if (sensors == null) sensors = GetComponent<AgentSensors>();
        if (energySystem == null) energySystem = GetComponent<AgentEnergySystem>();
    }

    public Vector3 ProcessInputs()
    {
        Genotype genotype = geneticController != null ? geneticController.genotype : null;
        currentIntention = SelectIntention(genotype);

        Vector3 desired = Vector3.zero;
        if (currentIntention.targetObject != null)
            desired += DirectionTo(currentIntention.targetObject.transform.position) * Mathf.Max(0.1f, currentIntention.utility);
        else if (currentIntention.targetPosition != Vector3.zero)
            desired += DirectionTo(currentIntention.targetPosition) * Mathf.Max(0.1f, currentIntention.utility);

        foreach (AgentSensors.SensorReading reading in sensors.readings)
        {
            float weight = GetWeightForType(reading.type, genotype);
            desired += reading.direction * (1f / Mathf.Max(reading.distance, 0.1f)) * weight;
        }

        float exploreWeight = genotype != null ? genotype.GetRange(Genotype.ExploreWeight, 0.02f, 0.35f) : 0.15f;
        Vector2 wander = Random.insideUnitCircle * exploreWeight;
        desired += new Vector3(wander.x, 0f, wander.y);
        desired.y = 0f;
        return desired.sqrMagnitude > 0.001f ? desired.normalized : transform.forward;
    }

    public bool ShouldBuild()
    {
        Genotype genotype = geneticController != null ? geneticController.genotype : null;
        if (genotype == null) return false;

        if (currentIntention.type != AgentIntentType.BuildUtility && currentIntention.type != AgentIntentType.BuildCommunity)
            return false;

        float buildWeight = genotype.Get(Genotype.BuildWeight);
        float innovation = genotype.Get(Genotype.InnovationDrive);
        return buildWeight > 0.55f && Random.value < Mathf.Lerp(0.006f, 0.025f, innovation);
    }

    public void Remember(WorldEvent worldEvent)
    {
        shortTermMemory.Enqueue(worldEvent);
        while (shortTermMemory.Count > MemoryLimit)
            shortTermMemory.Dequeue();
    }

    private AgentIntention SelectIntention(Genotype genotype)
    {
        AgentSensors.SensorReading nearestEnergy = FindNearest("Food", "EnergyCrystal");
        AgentSensors.SensorReading nearestBlock = FindNearest("Block", "Structure");
        AgentSensors.SensorReading nearestAgent = FindNearest("Agent", null);
        AgentSensors.SensorReading nearestHazard = FindNearest("Hazard", "Toxic");

        float energyRatio = energySystem != null && energySystem.maxEnergy > 0f ? energySystem.energy / energySystem.maxEnergy : 0.5f;
        float buildDrive = genotype != null ? genotype.Get(Genotype.BuildWeight) : 0.5f;
        float socialDrive = genotype != null ? genotype.Get(Genotype.SocialAffinity) : 0.4f;
        float curiosity = genotype != null ? genotype.Get(Genotype.CuriosityDrive) : 0.5f;
        float toolUse = genotype != null ? genotype.Get(Genotype.ToolUseDrive) : 0.5f;

        AgentIntention best = new AgentIntention(AgentIntentType.Explore, transform.position + transform.forward * 12f, null, curiosity);

        if (nearestHazard.target != null && nearestHazard.distance < 12f)
            best = ChooseBetter(best, new AgentIntention(AgentIntentType.AvoidHazard, transform.position - nearestHazard.direction * 12f, nearestHazard.target, 2f));

        if (nearestEnergy.target != null)
            best = ChooseBetter(best, new AgentIntention(AgentIntentType.SeekEnergy, nearestEnergy.target.transform.position, nearestEnergy.target, (1f - energyRatio) * 3f + 0.4f));

        if (nearestBlock.target != null)
            best = ChooseBetter(best, new AgentIntention(AgentIntentType.GatherBlock, nearestBlock.target.transform.position, nearestBlock.target, toolUse * 0.9f));

        if (nearestBlock.target != null && energyRatio > 0.45f)
            best = ChooseBetter(best, new AgentIntention(AgentIntentType.BuildUtility, nearestBlock.target.transform.position, nearestBlock.target, buildDrive * toolUse * 1.4f));

        if (nearestAgent.target != null && energyRatio > 0.35f)
        {
            float utility = socialDrive * Mathf.Lerp(0.6f, 1.8f, buildDrive);
            AgentIntentType type = buildDrive > 0.55f ? AgentIntentType.BuildCommunity : AgentIntentType.JoinGroup;
            best = ChooseBetter(best, new AgentIntention(type, nearestAgent.target.transform.position, nearestAgent.target, utility));
        }

        return best;
    }

    private AgentIntention ChooseBetter(AgentIntention current, AgentIntention candidate)
    {
        return candidate.utility > current.utility ? candidate : current;
    }

    private AgentSensors.SensorReading FindNearest(string typeA, string typeB)
    {
        AgentSensors.SensorReading best = new AgentSensors.SensorReading { distance = float.MaxValue };
        for (int i = 0; i < sensors.readings.Count; i++)
        {
            AgentSensors.SensorReading reading = sensors.readings[i];
            if (reading.target == null) continue;
            if (reading.type != typeA && (typeB == null || reading.type != typeB)) continue;
            if (reading.distance < best.distance) best = reading;
        }
        return best;
    }

    private Vector3 DirectionTo(Vector3 worldPosition)
    {
        Vector3 delta = worldPosition - transform.position;
        delta.y = 0f;
        return delta.sqrMagnitude > 0.001f ? delta.normalized : Vector3.zero;
    }

    private float GetWeightForType(string type, Genotype genotype)
    {
        if (genotype == null) return DefaultWeight(type);

        if (type == "EnergyCrystal") return genotype.GetRange(Genotype.ResourceWeight, 0.2f, 3f);
        if (type == "Food") return genotype.GetRange(Genotype.FoodWeight, 0.2f, 2.5f);
        if (type == "Block") return genotype.GetRange(Genotype.BlockWeight, -0.5f, 1.5f);
        if (type == "Structure") return genotype.GetRange(Genotype.StructureWeight, -0.5f, 1.5f);
        if (type == "Agent") return genotype.GetRange(Genotype.AgentWeight, -2f, 2f);
        if (type == "Hazard" || type == "Toxic" || type == "Wind") return -genotype.GetRange(Genotype.HazardWeight, 1f, 4f);
        return 0.2f;
    }

    private float DefaultWeight(string type)
    {
        if (type == "EnergyCrystal") return 1.5f;
        if (type == "Food") return 1.2f;
        if (type == "Hazard") return -2f;
        return 0.5f;
    }
}
