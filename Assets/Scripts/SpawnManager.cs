using UnityEngine;
using System.Collections.Generic;

// Singleton - non-persistent so each scene uses its own prefab references.
public class SpawnManager : Singleton<SpawnManager>
{
    protected override bool Persistent => false;

    [Header("Pickup Prefabs")]
    [Tooltip("Assign your Shield prefab here or place in Resources folder as 'Shield'.")]
    public GameObject shieldPrefab;

    [Tooltip("Assign your Boost prefab here or place in Resources folder as 'Boost'.")]
    public GameObject boostPrefab;

    private List<Vector3> _shieldSpawnPoints = new List<Vector3>();
    private List<Vector3> _boostSpawnPoints = new List<Vector3>();

    protected override void Awake()
    {
        base.Awake();
        CachePickupPositions();
    }

    private void CachePickupPositions()
    {
        GameObject[] shields = GameObject.FindGameObjectsWithTag("Shield");
        GameObject[] boosts = GameObject.FindGameObjectsWithTag("Boost");

        _shieldSpawnPoints.Clear();
        _boostSpawnPoints.Clear();

        foreach (var s in shields)
            if (s != null)
                _shieldSpawnPoints.Add(s.transform.position);

        foreach (var b in boosts)
            if (b != null)
                _boostSpawnPoints.Add(b.transform.position);
    }

    public void ResetPickups()
    {
        foreach (var s in GameObject.FindGameObjectsWithTag("Shield"))
            if (s != null)
                Destroy(s);

        foreach (var b in GameObject.FindGameObjectsWithTag("Boost"))
            if (b != null)
                Destroy(b);

        foreach (var pos in _shieldSpawnPoints)
            if (shieldPrefab != null)
                Instantiate(shieldPrefab, pos, Quaternion.identity);

        foreach (var pos in _boostSpawnPoints)
            if (boostPrefab != null)
                Instantiate(boostPrefab, pos, Quaternion.identity);
    }
}
