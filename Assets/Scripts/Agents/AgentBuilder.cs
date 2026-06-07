using UnityEngine;

public class AgentBuilder : MonoBehaviour
{
    public GeneticController geneticController;

    private GeneticAgent agent;
    private AgentEnergySystem energySystem;
    private float nextBuildTime;

    private void Awake()
    {
        agent = GetComponent<GeneticAgent>();
        energySystem = GetComponent<AgentEnergySystem>();
        if (geneticController == null) geneticController = GetComponent<GeneticController>();
    }

    public void AttemptBuild()
    {
        if (WorldManager.Instance == null || Time.time < nextBuildTime) return;

        WorldConfig config = WorldManager.Instance.config;
        Vector3 buildPos;
        if (!FindStableBuildPosition(config, out buildPos)) return;

        WorldObject block = WorldManager.Instance.SpawnBlock(buildPos, Quaternion.identity, agent.objectId);
        if (block == null) return;

        energySystem.ConsumeEnergy(config.BuildEnergyCost);
        nextBuildTime = Time.time + config.BuildCooldown;
        agent.NotifyStructureBuilt();
        WorldEventLogger.LogEvent("Build", agent.objectId, block.objectId, buildPos);
    }

    private bool FindStableBuildPosition(WorldConfig config, out Vector3 buildPos)
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 forward = transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(origin, forward, out hit, config.BuildDistance))
            buildPos = hit.point + hit.normal * 0.55f;
        else
            buildPos = transform.position + forward * config.BuildDistance;

        buildPos = new Vector3(Mathf.Round(buildPos.x), Mathf.Round(buildPos.y), Mathf.Round(buildPos.z));

        Collider[] overlaps = Physics.OverlapBox(buildPos, Vector3.one * 0.45f);
        if (overlaps.Length > 0) return false;

        Vector3 supportPoint = buildPos + Vector3.down * 0.6f;
        bool supported = Physics.CheckSphere(supportPoint, config.StabilityCheckRadius);
        if (!supported)
        {
            float ground = WorldManager.Instance.SampleTerrainHeight(buildPos);
            supported = Mathf.Abs(buildPos.y - ground) <= 1.5f;
            if (supported) buildPos.y = Mathf.Round(ground + 0.5f);
        }

        return supported;
    }
}
