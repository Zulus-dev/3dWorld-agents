using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DynamicZone : MonoBehaviour
{
    public enum ZoneType { Gravity, Toxic, Wind }
    public ZoneType type;
    public float strength = 1f;
    public float radius = 30f;

    private void Awake()
    {
        Collider zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        WorldObject obj = other.GetComponentInParent<WorldObject>();
        if (obj != null)
            WorldEventLogger.LogEvent("ZoneEnter", obj.objectId, gameObject.name, obj.transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (type == ZoneType.Gravity)
                rb.AddForce(Vector3.up * strength * -9.81f, ForceMode.Acceleration);
            else if (type == ZoneType.Wind)
                rb.AddForce(transform.forward * strength, ForceMode.Force);
        }

        if (type == ZoneType.Toxic)
        {
            AgentEnergySystem energySystem = other.GetComponentInParent<AgentEnergySystem>();
            if (energySystem != null)
                energySystem.ConsumeEnergy(Mathf.Abs(strength) * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WorldObject obj = other.GetComponentInParent<WorldObject>();
        if (obj != null)
            WorldEventLogger.LogEvent("ZoneExit", obj.objectId, gameObject.name, obj.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = type == ZoneType.Gravity ? Color.cyan : type == ZoneType.Toxic ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
