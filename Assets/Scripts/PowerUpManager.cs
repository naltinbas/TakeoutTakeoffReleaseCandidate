using UnityEngine;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpSpawn
    {
        public GameObject prefab;      // Boost or Shield prefab
        public Transform spawnPoint;   // Where it should appear
    }

    [SerializeField] private List<PowerUpSpawn> powerUps = new List<PowerUpSpawn>();

    private List<GameObject> _activePowerUps = new();

    void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        foreach (var entry in powerUps)
        {
            GameObject newPowerUp = Instantiate(entry.prefab, entry.spawnPoint.position, entry.spawnPoint.rotation);
            _activePowerUps.Add(newPowerUp);
        }
    }

    public void ResetPowerUps()
    {
        // Clear any old or destroyed ones
        foreach (var powerUp in _activePowerUps)
        {
            if (powerUp != null)
                Destroy(powerUp);
        }

        _activePowerUps.Clear();

        // Respawn fresh
        SpawnAll();
    }
}
