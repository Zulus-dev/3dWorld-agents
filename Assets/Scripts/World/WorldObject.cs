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
        ApplyRigidbodyDefaults();

        if (WorldManager.Instance != null)
            WorldManager.Instance.RegisterObject(this);
    }

    protected virtual void OnDestroy()
    {
        if (WorldManager.Instance != null)
            WorldManager.Instance.UnregisterObject(this);
    }

    protected virtual void Reset()
    {
        EnsureEditorPreviewData();
    }

    protected virtual void OnValidate()
    {
        EnsureEditorPreviewData();
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

        ApplyInferredDefaults(data);
    }

    private void EnsureEditorPreviewData()
    {
        if (Application.isPlaying || data != null) return;

        data = ScriptableObject.CreateInstance<WorldObjectData>();
        data.hideFlags = HideFlags.HideAndDontSave;
        ApplyInferredDefaults(data);
    }

    private void ApplyInferredDefaults(WorldObjectData targetData)
    {
        if (targetData == null) return;

        string inferredType = InferType();
        if (string.IsNullOrEmpty(targetData.type) || targetData.type == "Resource" || targetData.type == gameObject.name)
            targetData.type = inferredType;

        if (Mathf.Approximately(targetData.energy, 50f))
            targetData.energy = DefaultEnergyForType(targetData.type);
    }

    private void ApplyRigidbodyDefaults()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) return;

        rb.mass = Mathf.Clamp(rb.mass <= 0f ? 1f : rb.mass, 1f, 50f);
        rb.drag = Mathf.Clamp(rb.drag <= 0f ? 0.1f : rb.drag, 0.1f, 2f);
    }

    private string InferType()
    {
        if (GetComponent<GeneticAgent>() != null) return "Agent";

        string cleanName = gameObject.name.Replace("(Clone)", string.Empty).Trim();
        if (cleanName.Contains("EnergyCrystal")) return "EnergyCrystal";
        if (cleanName.Contains("Food")) return "Food";
        if (cleanName.Contains("Block")) return "Block";
        if (cleanName.Contains("Structure")) return "Structure";
        if (cleanName.Contains("Hazard")) return "Hazard";
        return cleanName;
    }

    private float DefaultEnergyForType(string type)
    {
        if (type == "EnergyCrystal") return 80f;
        if (type == "Food") return 35f;
        if (type == "Agent") return 100f;
        if (type == "Block" || type == "Structure") return 0f;
        return 50f;
    }
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
