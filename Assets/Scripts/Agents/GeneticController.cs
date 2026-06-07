using UnityEngine;
public class GeneticController : MonoBehaviour
{
    public Genotype genotype;
    public GeneticAgent agent;

    public void Initialize(Genotype parent1 = null, Genotype parent2 = null)
    {
        if (parent1 != null && parent2 != null)
        {
            genotype = parent1.Crossover(parent2);
        }
        else
        {
            genotype = new Genotype();
            genotype.InitializeRandomGenes(); // <-- ДОБАВЛЯЕМ СЮДА!
        }
        
        genotype.Mutate(WorldManager.Instance.config.MutationRate, WorldManager.Instance.config.MutationStrength);
    }
}