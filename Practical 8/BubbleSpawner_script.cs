using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float spawnRate = 10f; // per sec
    public float spawnRadius = 0.5f;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        float interval = 1f / spawnRate;
        if (timer >= interval)
        {
            SpawnBubble();
            timer = 0f;
        }
    }

    void SpawnBubble()
    {
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
        pos.y = transform.position.y; // keep spawn at same height
        GameObject b = Instantiate(bubblePrefab, pos, Quaternion.identity);
        float s = Random.Range(0.08f, 0.25f);
        b.transform.localScale = Vector3.one * s;
        // optionally randomize lifetime and buoyancy on Bubble component
        var c = b.GetComponent<Bubble>();
        if (c != null)
        {
            c.lifetime = Random.Range(2.5f, 6f);
            c.buoyancy = Random.Range(1.5f, 3.0f);
        }
    }
}
