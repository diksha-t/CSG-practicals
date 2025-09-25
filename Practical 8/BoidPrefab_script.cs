using UnityEngine;

public class Boid : MonoBehaviour
{
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public BoidManager manager;

    void Start()
    {
        // random initial velocity
        velocity = Random.onUnitSphere * manager.minSpeed;
        velocity.y = Mathf.Abs(velocity.y) * 0.3f; // bias upward a bit
    }

    void Update()
{
    // position update handled by manager (optional), but keep basic transform apply here
    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.MovePosition(transform.position + velocity * Time.deltaTime);
    }
    if (velocity.sqrMagnitude > 0.0001f)
        transform.forward = velocity.normalized;
}
// ...existing code...
}

