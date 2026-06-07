using UnityEngine;

public class DynamicZone : MonoBehaviour
{
    public enum ZoneType { Gravity, Toxic, Wind }
    public ZoneType type;
    public float strength = 1f;
    public float radius = 30f;

    private void OnTriggerStay(Collider other)
{
    // Заменяем TryGetComponent на обычный GetComponent с проверкой на null
    Rigidbody rb = other.GetComponent<Rigidbody>();
    
    if (rb != null)
    {
        if (type == ZoneType.Gravity)
            rb.AddForce(Vector3.up * strength * -9.81f, ForceMode.Acceleration);
        else if (type == ZoneType.Wind)
            rb.AddForce(transform.forward * strength, ForceMode.Force);
    }
}

    private void OnDrawGizmos()
    {
        Gizmos.color = type == ZoneType.Gravity ? Color.cyan : type == ZoneType.Toxic ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}