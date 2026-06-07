using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class GeneticAlgorithm
{
    public static void Evolve()
    {
        var agents = WorldManager.Instance.GetAgents();
        if (agents.Count < 20) return;

        // Сортируем по фитнесу
        List<GeneticAgent> sorted = agents.OrderByDescending(a => CalculateFitness(a)).ToList();

        int eliteCount = Mathf.Max(1, (int)(agents.Count * 0.1f));

        Debug.Log($"Evolution cycle: {agents.Count} agents, keeping {eliteCount} elites");

        // Здесь можно добавить создание новых агентов (пока заглушка)
        // В следующих шагах расширим
    }

    private static float CalculateFitness(GeneticAgent agent)
    {
        var cfg = WorldManager.Instance.config;
        return agent.energySystem.lifetime * cfg.LifetimeWeight +
               agent.energySystem.maxEnergyAchieved * cfg.EnergyWeight +
               /* structures + exploration + offspring */ 0f;
    }
}