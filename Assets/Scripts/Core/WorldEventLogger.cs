using UnityEngine;
using System.Collections.Generic;

public struct WorldEvent
{
    public float timestamp;
    public string eventType;
    public string objectId1;
    public string objectId2;
    public Vector3 position;
    public Dictionary<string, string> additionalData;
}

public class WorldEventLogger : MonoBehaviour
{
    public static void LogEvent(string type, string id1, string id2, Vector3 pos, Dictionary<string,string> data = null)
    {
        WorldEvent e = new WorldEvent
        {
            timestamp = Time.time,
            eventType = type,
            objectId1 = id1,
            objectId2 = id2,
            position = pos,
            additionalData = data ?? new Dictionary<string, string>()
        };

        if (ExternalWorldAdapter.Instance != null)
            ExternalWorldAdapter.Instance.PushEvent(e);
        else
            ExternalWorldAPI.PushEvent(e);
    }
}
