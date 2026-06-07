using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class GeneticAlgorithm
{
    public static void Evolve()
    {
        if (WorldManager.Instance == null || WorldManager.Instance.config == null) return;

        WorldConfig config = WorldManager.Instance.config;
        List<GeneticAgent> agents = WorldManager.Instance.GetAgents().Where(a => a != null && a.geneticController != null && a.geneticController.genotype != null).ToList();
        if (agents.Count == 0) return;

        List<GeneticAgent> sorted = agents.OrderByDescending(CalculateFitness).ToList();
        int eliteCount = Mathf.Max(1, Mathf.RoundToInt(sorted.Count * config.EliteRatio));
        int targetPopulation = Mathf.Max(config.MinimumPopulation, config.TargetPopulation);
        int missing = Mathf.Max(0, targetPopulation - agents.Count);

        for (int i = 0; i < missing; i++)
        {
            Genotype childGenotype;
            if (i < eliteCount && i < sorted.Count)
            {
                childGenotype = sorted[i].geneticController.genotype.Clone();
            }
            else
            {
                GeneticAgent parentA = TournamentSelect(sorted, config.TournamentSize);
                GeneticAgent parentB = TournamentSelect(sorted, config.TournamentSize);
                childGenotype = parentA.geneticController.genotype.Crossover(parentB.geneticController.genotype);
            }

            childGenotype.Mutate(config.MutationRate, config.MutationStrength);
            GeneticAgent anchor = sorted[Random.Range(0, sorted.Count)];
            Vector3 spawn = anchor.transform.position + Random.insideUnitSphere * 6f;
            spawn.y = WorldManager.Instance.SampleTerrainHeight(spawn) + config.SpawnHeightOffset;
            GeneticAgent child = WorldManager.Instance.SpawnAgent(childGenotype, spawn, false);
            if (child != null)
                WorldEventLogger.LogEvent("EvolutionBirth", anchor.objectId, child.objectId, spawn);
        }

        Debug.Log("Evolution cycle: " + agents.Count + " agents, elites=" + eliteCount + ", spawned=" + missing);
    }

    public static float CalculateFitness(GeneticAgent agent)
    {
        WorldConfig cfg = WorldManager.Instance.config;
        return agent.energySystem.lifetime * cfg.LifetimeWeight +
               agent.energySystem.maxEnergyAchieved * cfg.EnergyWeight +
               agent.StructuresBuiltCount * cfg.StructuresWeight +
               agent.NewChunksExplored * cfg.ExplorationWeight +
               agent.OffspringCount * cfg.OffspringWeight +
               (agent.developmentSystem != null ? agent.developmentSystem.DevelopmentScore * cfg.DevelopmentWeight : 0f);
    }

    private static GeneticAgent TournamentSelect(List<GeneticAgent> sortedAgents, int tournamentSize)
    {
        GeneticAgent best = null;
        float bestFitness = float.MinValue;
        int rounds = Mathf.Max(1, tournamentSize);

        for (int i = 0; i < rounds; i++)
        {
            GeneticAgent candidate = sortedAgents[Random.Range(0, sortedAgents.Count)];
            float fitness = CalculateFitness(candidate);
            if (best == null || fitness > bestFitness)
            {
                best = candidate;
                bestFitness = fitness;
            }
        }

        return best;
    }
}
