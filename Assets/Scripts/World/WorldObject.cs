using UnityEngine;

public class WorldObject : MonoBehaviour
{
    public string objectId;
    [SerializeField] public WorldObjectData data;   // ← Добавил [SerializeField] для надёжности

    protected virtual void Awake()
    {
        objectId = System.Guid.NewGuid().ToString();

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<WorldObjectData>();
        }

        if (WorldManager.Instance != null)
            WorldManager.Instance.RegisterObject(this);
    }

    protected virtual void OnDestroy()
    {
        if (WorldManager.Instance != null)
            WorldManager.Instance.UnregisterObject(this);
    }
}

[CreateAssetMenu(menuName = "Ecosystem/WorldObjectData")]
public class WorldObjectData : ScriptableObject
{
    public string type = "Resource";
    public float energy = 50f;
    public float age = 0f;
    public string ownerAgentId = "";
}