using UnityEngine;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class Genotype
{
    public float[] genes;

    // 1. Оставляем конструктор абсолютно ПУСТЫМ или просто выделяем память.
    // Unity сможет безопасно вызывать его при сериализации.
    public Genotype()
    {
        genes = new float[100];
    }

    // 2. Создаем отдельный метод для заполнения генов случайными числами
    public void InitializeRandomGenes()
    {
        for (int i = 0; i < genes.Length; i++)
            genes[i] = Random.Range(0f, 1f);
    }

    public Genotype Crossover(Genotype other)
    {
        Genotype child = new Genotype(); // Здесь создается пустой массив
        for (int i = 0; i < genes.Length; i++)
        {
            child.genes[i] = Random.value < 0.5f ? genes[i] : other.genes[i];
        }
        return child;
    }

    public void Mutate(float rate, float strength)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (Random.value < rate)
                genes[i] += Random.Range(-strength, strength);
        }
    }
}