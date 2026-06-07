using UnityEngine;

public class AgentBody : MonoBehaviour
{
    public GeneticController geneticController;
    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void ApplyGenotypeVisuals()
    {
        if (geneticController == null || geneticController.genotype == null) return;

        float size = Mathf.Lerp(0.8f, 1.8f, geneticController.genotype.genes[0]);
        transform.localScale = Vector3.one * size;

        Color color = Color.Lerp(Color.green, Color.blue, geneticController.genotype.genes[1]);
        foreach (var r in renderers)
            r.material.color = color;
    }
}