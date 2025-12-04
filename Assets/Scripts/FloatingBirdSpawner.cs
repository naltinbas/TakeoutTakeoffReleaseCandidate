using UnityEngine;

public class FloatingBirdSpawner : MonoBehaviour
{
    [Header("Cube Settings")]
    public GameObject cubePrefab;
    public int cubeCount = 5;
    public Vector3 cubeSize = Vector3.one;

    [Header("Oscillation Settings")]
    public float baseRadius = 3f;         // maximum horizontal distance from center
    public float oscillationSpeed = 1f;   // how fast they move in/out
    public float orbitSpeed = 1f;         // how fast they rotate around center
    public float verticalAmplitude = 2f;  // how high they oscillate upward
    public float noiseStrength = 0.5f;    // randomness

    [Header("Separation Settings")]
    public float minDistance = 1f;         // minimum spacing between birds
    public float separationForce = 2f;     // how strongly they repel each other

    [Header("Environment Settings")]
    public float maxHeight = 5f; // ceiling height relative to spawner

    private GameObject[] cubes;
    private float[] phaseOffsets;

    void Start()
    {
        cubes = new GameObject[cubeCount];
        phaseOffsets = new float[cubeCount];

        for (int i = 0; i < cubeCount; i++)
        {
            // give each cube a unique phase so they don’t overlap
            phaseOffsets[i] = Random.Range(0f, Mathf.PI * 2f);

            cubes[i] = Instantiate(cubePrefab, transform.position, Quaternion.identity);
            cubes[i].transform.localScale = cubeSize;
            cubes[i].transform.SetParent(transform);
        }
    }

    void Update()
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            if (cubes[i] == null) continue;

            float t = Time.time * oscillationSpeed + phaseOffsets[i];

            // oscillating horizontal radius
            float radius = Mathf.Sin(t) * baseRadius;

            // orbit on XZ plane
            float angle = Time.time * orbitSpeed + i * (2 * Mathf.PI / cubeCount);
            Vector3 orbit = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // vertical oscillation (always above spawner Y)
            float vertical = Mathf.Abs(Mathf.Sin(t)) * verticalAmplitude;

            // add noise
            float noiseX = (Mathf.PerlinNoise(Time.time, i * 0.1f) - 0.5f) * noiseStrength;
            float noiseY = (Mathf.PerlinNoise(i * 0.1f, Time.time) - 0.5f) * noiseStrength;
            float noiseZ = (Mathf.PerlinNoise(Time.time * 0.5f, i) - 0.5f) * noiseStrength;

            // base desired position (before separation)
            Vector3 desiredPos = transform.position
                                 + orbit * radius
                                 + new Vector3(noiseX, vertical + noiseY, noiseZ);

            // --- Separation check ---
            Vector3 separation = Vector3.zero;
            for (int j = 0; j < cubes.Length; j++)
            {
                if (i == j || cubes[j] == null) continue;

                Vector3 diff = cubes[i].transform.position - cubes[j].transform.position;
                float dist = diff.magnitude;

                if (dist < minDistance && dist > 0f)
                {
                    separation += diff.normalized * (minDistance - dist);
                }
            }

            // Apply separation force
            desiredPos += separation * separationForce * Time.deltaTime;

            // Clamp Y (never below spawner Y, never above spawner Y + maxHeight)
            float minY = transform.position.y + cubeSize.y * 0.5f;
            float maxY = transform.position.y + maxHeight;
            if (desiredPos.y < minY) desiredPos.y = minY;
            if (desiredPos.y > maxY) desiredPos.y = maxY;

            // Smooth movement
            cubes[i].transform.position = Vector3.Lerp(
                cubes[i].transform.position,
                desiredPos,
                Time.deltaTime * 5f
            );
        }
    }
}
