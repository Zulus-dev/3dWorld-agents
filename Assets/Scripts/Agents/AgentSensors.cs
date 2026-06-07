using UnityEngine;
using System.Collections.Generic;

public class AgentSensors : MonoBehaviour
{
    public GeneticController geneticController;
    public List<SensorReading> readings = new List<SensorReading>();

    private int rayCount = 8;
    private float sensorDistance = 50f;
    private float horizontalFov = 360f;
    private float verticalBias;

    public struct SensorReading
    {
        public string type;
        public Vector3 direction;
        public float distance;
        public WorldObject target;
    }

    private void Awake()
    {
        if (geneticController == null) geneticController = GetComponent<GeneticController>();
    }

    public void ApplyGenotype(Genotype genotype)
    {
        if (genotype == null) return;
        rayCount = genotype.GetInt(Genotype.SensorRayCount, 4, 12);
        sensorDistance = genotype.GetRange(Genotype.SensorDistance, 15f, 80f);
        horizontalFov = genotype.GetRange(Genotype.SensorHorizontalFov, 90f, 360f);
        verticalBias = genotype.GetRange(Genotype.SensorVerticalBias, -0.15f, 0.35f);
    }

    public void UpdateSensors()
    {
        readings.Clear();
        int count = Mathf.Max(1, rayCount);
        float startAngle = -horizontalFov * 0.5f;
        float step = count == 1 ? 0f : horizontalFov / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + step * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            dir = (dir + Vector3.up * verticalBias).normalized;

            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, dir, out hit, sensorDistance))
            {
                WorldObject wo = hit.collider.GetComponentInParent<WorldObject>();
                if (wo != null && wo.gameObject != gameObject)
                {
                    readings.Add(new SensorReading
                    {
                        type = wo.data != null ? wo.data.type : wo.gameObject.name,
                        direction = dir,
                        distance = Mathf.Max(hit.distance, 0.1f),
                        target = wo
                    });
                }
            }
        }
    }
}
