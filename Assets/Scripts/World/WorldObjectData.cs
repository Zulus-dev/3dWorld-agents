using UnityEngine;

[CreateAssetMenu(fileName = "WorldObjectData", menuName = "Ecosystem/WorldObjectData")]
public class WorldObjectData : ScriptableObject
{
    public string type = "Resource";
    public float energy = 50f;
    public float age = 0f;
    public string ownerAgentId = "";
}
