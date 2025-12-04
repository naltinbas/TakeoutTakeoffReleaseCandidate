using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    [Tooltip("Assign your Shield prefab here or place in Resources folder as 'Shield'.")]
    public GameObject shieldPrefab;

    [Tooltip("Assign your Boost prefab here or place in Resources folder as 'Boost'.")]
    public GameObject boostPrefab;

    private List<Vector3> shieldSpawnPoints = new List<Vector3>();
    private List<Vector3> boostSpawnPoints = new List<Vector3>();

    private static SpawnManager instance;

    void Awake()
    {
        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Listen for scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initial cache for the first scene
        CachePickupPositions();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Each time a new scene loads, update pickup positions
        CachePickupPositions();
        ResetPickups();
    }

    private void CachePickupPositions()
    {
        GameObject[] shields = GameObject.FindGameObjectsWithTag("Shield");
        GameObject[] boosts = GameObject.FindGameObjectsWithTag("Boost");

        shieldSpawnPoints.Clear();
        boostSpawnPoints.Clear();

        foreach (var s in shields)
            if (s != null)
                shieldSpawnPoints.Add(s.transform.position);

        foreach (var b in boosts)
            if (b != null)
                boostSpawnPoints.Add(b.transform.position);
    }

    public void ResetPickups()
    {
        foreach (var s in GameObject.FindGameObjectsWithTag("Shield"))
            if (s != null)
                Destroy(s);

        foreach (var b in GameObject.FindGameObjectsWithTag("Boost"))
            if (b != null)
                Destroy(b);

        foreach (var pos in shieldSpawnPoints)
            if (shieldPrefab != null)
                Instantiate(shieldPrefab, pos, Quaternion.identity);

        foreach (var pos in boostSpawnPoints)
            if (boostPrefab != null)
                Instantiate(boostPrefab, pos, Quaternion.identity);
    }
}
