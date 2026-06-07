using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentPhysics : MonoBehaviour
{
    private Rigidbody rb;
    public float baseSpeed = 8f;
    public float jumpForce = 12f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 5f;
        rb.drag = 0.4f;
    }

    public void ApplyGenotype(Genotype genotype)
    {
        if (genotype == null) return;
        baseSpeed = genotype.GetRange(Genotype.BaseSpeed, 3f, 14f);
        jumpForce = genotype.GetRange(Genotype.JumpForce, 6f, 18f);
        rb.mass = genotype.GetRange(Genotype.Mass, 1f, 25f);
        rb.drag = genotype.GetRange(Genotype.Drag, 0.1f, 2f);
    }

    public void ApplyMovement(Vector3 desiredDirection)
    {
        Vector3 planar = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
        if (planar.sqrMagnitude > 1f) planar.Normalize();

        rb.AddForce(planar * baseSpeed * 50f, ForceMode.Force);

        Vector3 planarVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float maxSpeed = baseSpeed * 1.5f;
        if (planarVelocity.magnitude > maxSpeed)
            rb.velocity = planarVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        if (desiredDirection.y > 0.65f) Jump();
    }

    public void Jump()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.2f))
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
