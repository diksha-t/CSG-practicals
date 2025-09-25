using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float lifetime = 5f;
    public float buoyancy = 2f; // upward force
    public float jitter = 0.5f;

    Rigidbody rb;
    float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        timer = Random.Range(0f, 0.5f); // tiny offset
    }

    void FixedUpdate()
    {
        // Buoyancy: upward force proportional to bubble volume (approx)
        rb.AddForce(Vector3.up * buoyancy * Time.fixedDeltaTime, ForceMode.Acceleration);

        // Small lateral jitter
        Vector3 jitterVec = new Vector3(Mathf.PerlinNoise(Time.time + transform.position.x, 0f)-0.5f,
                                        0f,
                                        Mathf.PerlinNoise(Time.time + transform.position.z, 1f)-0.5f);
        rb.AddForce(jitterVec * jitter * Time.fixedDeltaTime, ForceMode.Acceleration);

        // optional slight expansion
        // transform.localScale += Vector3.one * 0.0005f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifetime) Pop();
    }

    void OnCollisionEnter(Collision col)
    {
        // optional: slow down or bounce
    }

    void Pop()
    {
        // TODO: spawn small splash/particle effect here
        Destroy(gameObject);
    }
}
