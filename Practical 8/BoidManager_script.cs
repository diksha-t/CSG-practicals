using UnityEngine;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour
{
    [Header("Prefabs & spawn")]
    public GameObject boidPrefab;
    public int boidCount = 120;
    public float spawnRadius = 6f;

    [Header("Behavior")]
    public float neighborRadius = 1.8f;
    public float separationRadius = 0.6f;

    public float maxSpeed = 3f;
    public float minSpeed = 1f;
    public float maxForce = 3f; // steering limit

    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float targetWeight = 0.6f;

    public Transform target; // optional attractor

    [HideInInspector] public List<Boid> boids = new List<Boid>();

    void Start()
    {
        for (int i=0; i<boidCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            pos.y = Mathf.Clamp(pos.y, 0.2f, spawnRadius); // keep above ground a bit
            GameObject go = Instantiate(boidPrefab, pos, Random.rotation);
            Boid b = go.GetComponent<Boid>();
            b.manager = this;
            boids.Add(b);
        }
    }

    void Update()
    {
        for (int i=0; i<boids.Count; i++)
        {
            Boid b = boids[i];
            Vector3 cohesion = Vector3.zero;
            Vector3 alignment = Vector3.zero;
            Vector3 separation = Vector3.zero;
            int cohesionCount = 0;
            int alignCount = 0;
            int sepCount = 0;

            // neighbor loop (O(n^2) — fine for ~200 boids)
            for (int j=0; j<boids.Count; j++)
            {
                if (i==j) continue;
                Boid other = boids[j];
                Vector3 offset = other.transform.position - b.transform.position;
                float dist = offset.magnitude;
                if (dist < neighborRadius)
                {
                    cohesion += other.transform.position;
                    alignment += other.velocity;
                    cohesionCount++; alignCount++;
                }
                if (dist < separationRadius && dist > 0.0001f)
                {
                    separation -= (offset / (dist * dist)); // weighted away
                    sepCount++;
                }
            }

            if (cohesionCount > 0) cohesion = ((cohesion / cohesionCount) - b.transform.position).normalized * maxSpeed;
            if (alignCount > 0) alignment = (alignment / alignCount).normalized * maxSpeed;
            if (sepCount > 0) separation = separation.normalized * maxSpeed;

            // steering = desired - currentVelocity
            Vector3 steer = Vector3.zero;
            steer += (cohesion - b.velocity) * cohesionWeight;
            steer += (alignment - b.velocity) * alignmentWeight;
            steer += (separation - b.velocity) * separationWeight;

            // target attraction
            if (target != null)
            {
                Vector3 toTarget = (target.position - b.transform.position).normalized * maxSpeed;
                steer += (toTarget - b.velocity) * targetWeight;
            }

            // clamp steering
            steer = Vector3.ClampMagnitude(steer, maxForce);

            // integrate velocity
            b.velocity = Vector3.ClampMagnitude(b.velocity + steer * Time.deltaTime, maxSpeed);

            // enforce min speed (optional, prevents stopping)
            float speed = b.velocity.magnitude;
            if (speed < minSpeed) b.velocity = b.velocity.normalized * minSpeed;
        }
    }
}
