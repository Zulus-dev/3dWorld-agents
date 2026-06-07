using UnityEngine;
using System.Collections.Generic;

public class AgentSensors : MonoBehaviour
{
    public GeneticController geneticController;
    public List<SensorReading> readings = new List<SensorReading>();

    public struct SensorReading
    {
        public string type;
        public Vector3 direction;
        public float distance;
        public WorldObject target;
    }

    public void UpdateSensors()
    {
        readings.Clear();
        int rayCount = 8; // можно читать из генотипа

        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            if (Physics.Raycast(transform.position + Vector3.up * 1f, dir, out RaycastHit hit, 50f))
            {
                WorldObject wo = hit.collider.GetComponent<WorldObject>();
                if (wo != null)
                {
                    readings.Add(new SensorReading
                    {
                        type = wo.data.type,
                        direction = dir,
                        distance = hit.distance,
                        target = wo
                    });
                }
            }
        }
    }
}