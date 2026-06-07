using UnityEngine;

[CreateAssetMenu(fileName = "WorldConfig", menuName = "Ecosystem/WorldConfig")]
public class WorldConfig : ScriptableObject
{
    [Header("World Settings")]
    public Vector3 WorldSize = new Vector3(500, 300, 500);
    public int InitialAgents = 80;
    public int InitialResources = 150;
    public float EvolutionInterval = 1200f; // тики
    public float MutationRate = 0.12f;
    public float MutationStrength = 0.25f;

    [Header("Fitness Weights")]
    public float LifetimeWeight = 0.4f;
    public float EnergyWeight = 0.25f;
    public float StructuresWeight = 0.2f;
    public float ExplorationWeight = 0.1f;
    public float OffspringWeight = 0.05f;

    [Header("Agent Settings")]
    public float BaseMetabolism = 0.15f;
    public float EnergyDeathThreshold = 0f;
    public float ReproductionEnergyThreshold = 80f;
}