using UnityEngine;

public enum AgentIntentType
{
    Explore,
    SeekEnergy,
    GatherBlock,
    BuildUtility,
    BuildCommunity,
    JoinGroup,
    AvoidHazard
}

[System.Serializable]
public struct AgentIntention
{
    public AgentIntentType type;
    public Vector3 targetPosition;
    public WorldObject targetObject;
    public float utility;

    public AgentIntention(AgentIntentType type, Vector3 targetPosition, WorldObject targetObject, float utility)
    {
        this.type = type;
        this.targetPosition = targetPosition;
        this.targetObject = targetObject;
        this.utility = utility;
    }
}
