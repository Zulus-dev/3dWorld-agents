using UnityEngine;

public class GeneticController : MonoBehaviour
{
    public Genotype genotype;
    public GeneticAgent agent;

    public void Initialize(Genotype inheritedGenotype = null, bool mutate = true)
    {
        agent = agent == null ? GetComponent<GeneticAgent>() : agent;

        if (inheritedGenotype != null)
            genotype = inheritedGenotype.Clone();
        else if (genotype == null || genotype.genes == null || genotype.genes.Length == 0)
            genotype = Genotype.CreateRandom();

        if (mutate && WorldManager.Instance != null && WorldManager.Instance.config != null)
            genotype.Mutate(WorldManager.Instance.config.MutationRate, WorldManager.Instance.config.MutationStrength);
    }
}
