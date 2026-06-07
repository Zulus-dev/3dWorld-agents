using UnityEngine;

public class ExternalWorldAdapter : MonoBehaviour
{
    public static ExternalWorldAdapter Instance;

    private void Awake() => Instance = this;

    public void RegisterAllObjects()
    {
        // Вызывается после спавна
        Debug.Log("All objects registered to API");
        // Здесь будет ExternalWorldAPI.RegisterObject(...)
    }

    public void PushEvent(WorldEvent e)
    {
        // ExternalWorldAPI.PushEvent(e);
        Debug.Log($"Event: {e.eventType} at {e.position}");
    }

    public void PushWorldSnapshot()
    {
        // Compact snapshot каждые N тиков
    }
}