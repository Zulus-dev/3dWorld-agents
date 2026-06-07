using UnityEngine;

public class AgentEnergySystem : MonoBehaviour
{
    public float energy = 100f;
    public float maxEnergy = 150f;
    public float lifetime;
    public float maxEnergyAchieved;

    private GeneticAgent agent;
    private float metabolismRate;
    private float maxAge;

    private void Awake()
    {
        agent = GetComponent<GeneticAgent>();
    }

    private void FixedUpdate()
    {
        if (WorldManager.Instance == null || WorldManager.Instance.config == null) return;

        lifetime += Time.fixedDeltaTime;
        energy -= metabolismRate * Time.fixedDeltaTime;
        maxEnergyAchieved = Mathf.Max(maxEnergyAchieved, energy);

        if (energy <= WorldManager.Instance.config.EnergyDeathThreshold || lifetime >= maxAge)
            agent.Die();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryConsumeResource(collision.collider.GetComponentInParent<WorldObject>());
    }

    private void OnTriggerEnter(Collider other)
    {
        TryConsumeResource(other.GetComponentInParent<WorldObject>());
    }

    public void ApplyGenotype(Genotype genotype)
    {
        WorldConfig config = WorldManager.Instance.config;
        metabolismRate = config.BaseMetabolism;
        maxAge = config.MaxAgentAge;

        if (genotype != null)
        {
            metabolismRate *= genotype.GetRange(Genotype.MetabolismRate, 0.5f, 2.5f);
            maxEnergy = genotype.GetRange(Genotype.MaxEnergy, 100f, 250f);
            maxAge = genotype.GetRange(Genotype.MaxAge, config.MaxAgentAge * 0.5f, config.MaxAgentAge * 1.5f);
        }

        energy = Mathf.Clamp(energy, 1f, maxEnergy);
        maxEnergyAchieved = energy;
    }

    public void ConsumeEnergy(float amount = 0f)
    {
        energy -= Mathf.Max(0f, amount);
    }

    public bool CanReproduce()
    {
        if (WorldManager.Instance == null || WorldManager.Instance.config == null) return false;
        float threshold = WorldManager.Instance.config.ReproductionEnergyThreshold;
        Genotype genotype = agent != null && agent.geneticController != null ? agent.geneticController.genotype : null;
        if (genotype != null)
            threshold = genotype.GetRange(Genotype.ReproductionThreshold, threshold * 0.8f, maxEnergy * 0.95f);
        return energy > threshold;
    }

    public void AddEnergy(float amount)
    {
        energy = Mathf.Min(maxEnergy, energy + amount);
        maxEnergyAchieved = Mathf.Max(maxEnergyAchieved, energy);
    }

    private void TryConsumeResource(WorldObject resource)
    {
        if (resource == null || resource == agent || resource.data == null) return;
        if (resource.data.type != "Food" && resource.data.type != "EnergyCrystal") return;

        AddEnergy(resource.data.energy);
        if (agent != null) agent.NotifyResourceCollected(resource.data.type);
        WorldEventLogger.LogEvent("ResourceCollected", agent.objectId, resource.objectId, resource.transform.position);
        Destroy(resource.gameObject);
    }
}
