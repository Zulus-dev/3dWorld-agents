using UnityEngine;
using System.Collections.Generic;

public class AgentBrain : MonoBehaviour
{
    public GeneticController geneticController;
    public AgentSensors sensors;

    private readonly Queue<WorldEvent> shortTermMemory = new Queue<WorldEvent>();
    private const int MemoryLimit = 10;

    private void Awake()
    {
        if (geneticController == null) geneticController = GetComponent<GeneticController>();
        if (sensors == null) sensors = GetComponent<AgentSensors>();
    }

    public Vector3 ProcessInputs()
    {
        Vector3 desired = Vector3.zero;
        Genotype genotype = geneticController != null ? geneticController.genotype : null;

        foreach (AgentSensors.SensorReading reading in sensors.readings)
        {
            float weight = GetWeightForType(reading.type, genotype);
            desired += reading.direction * (1f / Mathf.Max(reading.distance, 0.1f)) * weight;
        }

        float exploreWeight = genotype != null ? genotype.GetRange(Genotype.ExploreWeight, 0.05f, 0.8f) : 0.3f;
        desired += Random.insideUnitSphere * exploreWeight;
        desired.y = Mathf.Clamp(desired.y, -0.25f, 1f);
        return desired.sqrMagnitude > 0.001f ? desired.normalized : transform.forward;
    }

    public bool ShouldBuild()
    {
        Genotype genotype = geneticController != null ? geneticController.genotype : null;
        if (genotype == null) return false;

        float buildWeight = genotype.Get(Genotype.BuildWeight);
        return buildWeight > 0.65f && Random.value < buildWeight * 0.02f;
    }

    public void Remember(WorldEvent worldEvent)
    {
        shortTermMemory.Enqueue(worldEvent);
        while (shortTermMemory.Count > MemoryLimit)
            shortTermMemory.Dequeue();
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
