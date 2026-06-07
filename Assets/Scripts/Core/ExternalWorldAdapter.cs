using UnityEngine;

public class ExternalWorldAdapter : MonoBehaviour
{
    public static ExternalWorldAdapter Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterAllObjects()
    {
        if (WorldManager.Instance == null) return;

        foreach (WorldObject worldObject in WorldManager.Instance.GetWorldObjects())
            RegisterObject(worldObject);
    }

    public void RegisterObject(WorldObject worldObject)
    {
        if (worldObject == null || worldObject.data == null) return;
        ExternalWorldAPI.RegisterObject(worldObject.objectId, worldObject.data.type, worldObject.CreateSnapshot());
    }

    public void PushEvent(WorldEvent e)
    {
        ExternalWorldAPI.PushEvent(e);

        WorldConfig config = WorldManager.Instance != null ? WorldManager.Instance.config : null;
        if (config != null && config.LogEventsToConsole)
            Debug.Log("Event: " + e.eventType + " at " + e.position);
    }

    public void PushWorldSnapshot()
    {
        if (WorldManager.Instance != null)
            ExternalWorldAPI.PushWorldSnapshot(WorldManager.Instance.CreateCompactSnapshot());
    }
}
