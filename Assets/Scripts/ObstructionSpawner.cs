using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject birdsPrefab;
    [SerializeField] private GameObject cloudsPrefab;
    private GameObject _player;

    private readonly List<GameObject> _activeObstructions = new();

    [Header("Base Speeds")]
    [SerializeField] private float cloudSpeed = 1f;
    [SerializeField] private float birdSpeed = 3f;

    [Header("Spawner Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 5f;
    [SerializeField] private bool randomizeSpawn = false;

    private IEnumerator RepeatAction()
    {
        while (true)
        {
            float waitTime = randomizeSpawn
                ? Random.Range(minSpawnInterval, maxSpawnInterval)
                : spawnInterval;

            yield return new WaitForSeconds(waitTime);

            float probability = Random.Range(0f, 1f);
            GenerateObstruction(probability < 0.5f ? cloudsPrefab : birdsPrefab);
        }
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        // Strategy pattern - load from data config instead of hardcoded switch
        ApplyDifficultySettings(GameManager.CurrentDifficulty);

        StartCoroutine(RepeatAction());
    }

    private void Update()
    {
        for (int i = _activeObstructions.Count - 1; i >= 0; i--)
        {
            GameObject obj = _activeObstructions[i];
            if (obj == null)
            {
                _activeObstructions.RemoveAt(i);
                continue;
            }

            float speed = obj.CompareTag("Cloud") ? cloudSpeed : birdSpeed;
            var backDirection = -_player.transform.forward;
            obj.transform.Translate(backDirection * (speed * Time.deltaTime), Space.World);

            var proj = Vector3.Project(obj.transform.position - _player.transform.position, backDirection);

            if (proj.magnitude > 15f && Vector3.Dot(obj.transform.position - _player.transform.position, backDirection) > 0f)
            {
                Destroy(obj);
                _activeObstructions.RemoveAt(i);
            }
        }
    }

    private void GenerateObstruction(GameObject prefab)
    {
        float offsetZ = Mathf.Sign(_player.transform.forward.z) * Random.Range(7.5f, 50f),
            offsetX = Mathf.Abs(_player.transform.forward.x) > 0.5f ? Mathf.Sign(_player.transform.forward.x) * Random.Range(7.5f, 50f) : 0;
        Vector3 pos = _player.transform.position + new Vector3(offsetX, 0, offsetZ);
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        _activeObstructions.Add(obj);
    }

    private void ApplyDifficultySettings(Difficulty difficulty)
    {
        var config = DifficultyConfig.GetConfig(difficulty);
        cloudSpeed = config.CloudSpeed;
        birdSpeed = config.BirdSpeed;
        spawnInterval = config.SpawnInterval;
        minSpawnInterval = config.MinSpawnInterval;
        maxSpawnInterval = config.MaxSpawnInterval;
    }
}
