using UnityEngine;

public class AgentEnergySystem : MonoBehaviour
{
    public float energy = 100f;
    public float maxEnergy = 150f;
    public float lifetime = 0f;
    public float maxEnergyAchieved = 0f;

    private GeneticAgent agent;

    private void Awake()
    {
        agent = GetComponent<GeneticAgent>();
    }

    private void FixedUpdate()
    {
        lifetime += Time.fixedDeltaTime;
        energy -= WorldManager.Instance.config.BaseMetabolism * Time.fixedDeltaTime;

        if (energy > maxEnergyAchieved) maxEnergyAchieved = energy;

        if (energy <= WorldManager.Instance.config.EnergyDeathThreshold)
            agent.Die();
    }

    public void ConsumeEnergy() { /* уже в FixedUpdate */ }

    public bool CanReproduce() => energy > WorldManager.Instance.config.ReproductionEnergyThreshold;

    public void AddEnergy(float amount)
    {
        energy = Mathf.Min(maxEnergy, energy + amount);
    }
}