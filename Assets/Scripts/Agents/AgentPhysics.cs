using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentPhysics : MonoBehaviour
{
    private Rigidbody rb;
    public float baseSpeed = 8f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 5f;
    }

    public void ApplyMovement(Vector3 desiredDirection)
    {
        Vector3 force = desiredDirection * baseSpeed * 50f;
        rb.AddForce(force, ForceMode.Force);

        // Ограничение скорости
        if (rb.velocity.magnitude > baseSpeed * 1.5f)
            rb.velocity = rb.velocity.normalized * baseSpeed * 1.5f;
    }

    public void Jump()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.2f))
            rb.AddForce(Vector3.up * 12f, ForceMode.Impulse);
    }
}