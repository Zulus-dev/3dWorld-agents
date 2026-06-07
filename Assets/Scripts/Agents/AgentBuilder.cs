using UnityEngine;

public class AgentBuilder : MonoBehaviour
{
    public GeneticController geneticController;

    public void AttemptBuild()
    {
        // Простая реализация — ставит блок перед собой
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Block") || hit.collider.CompareTag("Structure"))
            {
                Vector3 buildPos = hit.point + hit.normal * 0.5f;
                Instantiate(Resources.Load<GameObject>("Prefabs/Block"), buildPos, Quaternion.identity);
                WorldEventLogger.LogEvent("Build", GetComponent<WorldObject>().objectId, "", buildPos);
            }
        }
    }
}