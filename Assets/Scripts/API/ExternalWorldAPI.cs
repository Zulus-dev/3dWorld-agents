using UnityEngine;
using System.Collections.Generic;

public static class ExternalWorldAPI
{
    public static readonly List<WorldEvent> Events = new List<WorldEvent>();
    public static readonly Dictionary<string, WorldObjectSnapshot> Objects = new Dictionary<string, WorldObjectSnapshot>();
    public static readonly List<CompactWorldSnapshot> Snapshots = new List<CompactWorldSnapshot>();

    public static void RegisterObject(string id, string type, object snapshot)
    {
        WorldObjectSnapshot typedSnapshot = snapshot is WorldObjectSnapshot ? (WorldObjectSnapshot)snapshot : new WorldObjectSnapshot { objectId = id, type = type };
        Objects[id] = typedSnapshot;
    }

    public static void PushEvent(WorldEvent e)
    {
        Events.Add(e);
    }

    public static void PushWorldSnapshot(CompactWorldSnapshot snapshot)
    {
        Snapshots.Add(snapshot);
    }

    public static void PushWorldSnapshot()
    {
        if (WorldManager.Instance != null)
            PushWorldSnapshot(WorldManager.Instance.CreateCompactSnapshot());
    }
}

[System.Serializable]
public struct TriadicObservation
{
    public string objectId;
    public int state;
    public Vector3 position;
    public Vector3 velocity;
    public float energyLevel;
    public float confidence;
    public int patternHash;
}

[System.Serializable]
public struct CompactWorldSnapshot
{
    public float timestamp;
    public int agentCount;
    public int objectCount;
    public TriadicObservation[] observations;
}
