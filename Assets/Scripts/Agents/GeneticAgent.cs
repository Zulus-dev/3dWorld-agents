using UnityEngine;

public class GeneticAgent : WorldObject
{
    public GeneticController geneticController;
    public AgentPhysics physics;
    public AgentSensors sensors;
    public AgentBrain brain;
    public AgentEnergySystem energySystem;
    public AgentBuilder builder;

    private void Start()
    {
        geneticController = GetComponent<GeneticController>();
        physics = GetComponent<AgentPhysics>();
        sensors = GetComponent<AgentSensors>();
        brain = GetComponent<AgentBrain>();
        energySystem = GetComponent<AgentEnergySystem>();
        builder = GetComponent<AgentBuilder>();

        geneticController.agent = this;
        geneticController.Initialize();
    }

    private void FixedUpdate()
    {
        sensors.UpdateSensors();
        Vector3 desiredMove = brain.ProcessInputs();
        physics.ApplyMovement(desiredMove);
        energySystem.ConsumeEnergy();

        if (energySystem.CanReproduce()) AttemptReproduce();
    }

    private void AttemptReproduce()
    {
        // Создать нового агента рядом
        Vector3 spawnPos = transform.position + Random.insideUnitSphere * 3f;
        spawnPos.y = 5f;
        Instantiate(Resources.Load<GameObject>("Prefabs/GeneticAgent"), spawnPos, Quaternion.identity);
        energySystem.energy -= 40f; // стоимость размножения
        WorldEventLogger.LogEvent("Reproduction", objectId, "newAgent", spawnPos);
    }

    public void Die()
    {
        // Оставить тело как ресурс
        WorldEventLogger.LogEvent("Death", objectId, "", transform.position);
        Destroy(gameObject, 0.1f);
    }
}