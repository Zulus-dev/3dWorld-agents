using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldObject : MonoBehaviour
{
    public string objectId;
    [SerializeField] public WorldObjectData data;

    protected virtual void Awake()
    {
        objectId = System.Guid.NewGuid().ToString();
        EnsureDataInstance();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = Mathf.Clamp(rb.mass <= 0f ? 1f : rb.mass, 1f, 50f);
            rb.drag = Mathf.Clamp(rb.drag, 0.1f, 2f);
        }

        if (WorldManager.Instance != null)
            WorldManager.Instance.RegisterObject(this);
    }

    protected virtual void OnDestroy()
    {
        if (WorldManager.Instance != null)
            WorldManager.Instance.UnregisterObject(this);
    }

    public virtual WorldObjectSnapshot CreateSnapshot()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        return new WorldObjectSnapshot
        {
            objectId = objectId,
            type = data != null ? data.type : gameObject.name,
            position = transform.position,
            velocity = rb != null ? rb.velocity : Vector3.zero,
            energy = data != null ? data.energy : 0f,
            age = data != null ? data.age : 0f,
            ownerAgentId = data != null ? data.ownerAgentId : string.Empty
        };
    }

    protected void EnsureDataInstance()
    {
        if (data == null)
            data = ScriptableObject.CreateInstance<WorldObjectData>();
        else
            data = Instantiate(data);

        if (string.IsNullOrEmpty(data.type) || data.type == "Resource")
            data.type = InferType();
    }

    private string InferType()
    {
        if (CompareTag("Block")) return "Block";
        if (CompareTag("Resource")) return gameObject.name.Replace("(Clone)", string.Empty);
        if (CompareTag("Structure")) return "Structure";
        if (GetComponent<GeneticAgent>() != null) return "Agent";
        return gameObject.name.Replace("(Clone)", string.Empty);
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

[System.Serializable]
public struct WorldObjectSnapshot
{
    public string objectId;
    public string type;
    public Vector3 position;
    public Vector3 velocity;
    public float energy;
    public float age;
    public string ownerAgentId;
}
