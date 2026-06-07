using UnityEngine;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class Genotype
{
    public const int GeneCount = 100;

    public const int BodySize = 0;
    public const int BodyHue = 1;
    public const int SensorRayCount = 2;
    public const int SensorDistance = 3;
    public const int SensorHorizontalFov = 4;
    public const int SensorVerticalBias = 5;

    public const int ResourceWeight = 10;
    public const int FoodWeight = 11;
    public const int BlockWeight = 12;
    public const int AgentWeight = 13;
    public const int HazardWeight = 14;
    public const int StructureWeight = 15;
    public const int ExploreWeight = 16;
    public const int SeparationWeight = 17;
    public const int BuildWeight = 18;

    public const int BaseSpeed = 30;
    public const int JumpForce = 31;
    public const int Mass = 32;
    public const int Drag = 33;
    public const int MetabolismRate = 34;
    public const int MaxEnergy = 35;
    public const int ReproductionThreshold = 36;
    public const int MaxAge = 37;

    public const int PreferredStructureHeight = 50;
    public const int BridgeTendency = 51;
    public const int TowerStabilityPreference = 52;

    public float[] genes;

    public Genotype()
    {
        genes = new float[GeneCount];
    }

    public static Genotype CreateRandom()
    {
        Genotype genotype = new Genotype();
        genotype.InitializeRandomGenes();
        return genotype;
    }

    public Genotype Clone()
    {
        Genotype clone = new Genotype();
        Array.Copy(genes, clone.genes, GeneCount);
        return clone;
    }

    public void InitializeRandomGenes()
    {
        EnsureLength();
        for (int i = 0; i < genes.Length; i++)
            genes[i] = Random.Range(0f, 1f);
    }

    public float Get(int index, float fallback = 0.5f)
    {
        EnsureLength();
        if (index < 0 || index >= genes.Length) return fallback;
        return Mathf.Clamp01(genes[index]);
    }

    public int GetInt(int index, int min, int max)
    {
        return Mathf.RoundToInt(Mathf.Lerp(min, max, Get(index)));
    }

    public float GetRange(int index, float min, float max)
    {
        return Mathf.Lerp(min, max, Get(index));
    }

    public Genotype Crossover(Genotype other)
    {
        EnsureLength();
        other.EnsureLength();

        Genotype child = new Genotype();
        for (int i = 0; i < genes.Length; i++)
            child.genes[i] = Random.value < 0.5f ? genes[i] : other.genes[i];
        return child;
    }

    public void Mutate(float rate, float strength)
    {
        EnsureLength();
        for (int i = 0; i < genes.Length; i++)
        {
            if (Random.value < rate)
                genes[i] = Mathf.Clamp01(genes[i] + Random.Range(-strength, strength));
        }
    }

    private void EnsureLength()
    {
        if (genes == null)
        {
            genes = new float[GeneCount];
            return;
        }

        if (genes.Length == GeneCount) return;

        float[] resized = new float[GeneCount];
        Array.Copy(genes, resized, Mathf.Min(genes.Length, resized.Length));
        genes = resized;
    }
}
