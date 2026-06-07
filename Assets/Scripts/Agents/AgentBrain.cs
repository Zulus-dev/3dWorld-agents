using UnityEngine;

public class AgentBrain : MonoBehaviour
{
    public GeneticController geneticController;
    public AgentSensors sensors;

    public Vector3 ProcessInputs()
    {
        Vector3 desired = Vector3.zero;

        foreach (var reading in sensors.readings)
        {
            float weight = GetWeightForType(reading.type);
            desired += reading.direction * (1f / reading.distance) * weight;
        }

        // Добавляем небольшое случайное исследование
        desired += Random.insideUnitSphere * 0.3f;
        return desired.normalized;
    }

    private float GetWeightForType(string type)
    {
        // Здесь можно расширять через гены
        if (type == "EnergyCrystal") return 1.5f;
        if (type == "Food") return 1.2f;
        if (type == "Hazard") return -2f;
        return 0.5f;
    }
}