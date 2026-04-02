using System.Collections.Generic;

// Strategy pattern - data-driven difficulty parameters.
[System.Serializable]
public class DifficultyConfig
{
    public float CloudSpeed;
    public float BirdSpeed;
    public float SpawnInterval;
    public float MinSpawnInterval;
    public float MaxSpawnInterval;

    private static readonly Dictionary<Difficulty, DifficultyConfig> Configs = new()
    {
        {
            Difficulty.Easy, new DifficultyConfig
            {
                CloudSpeed = 1f, BirdSpeed = 2f,
                SpawnInterval = 4f, MinSpawnInterval = 3f, MaxSpawnInterval = 5.5f
            }
        },
        {
            Difficulty.Medium, new DifficultyConfig
            {
                CloudSpeed = 2.5f, BirdSpeed = 3.5f,
                SpawnInterval = 2.5f, MinSpawnInterval = 2f, MaxSpawnInterval = 4.5f
            }
        },
        {
            Difficulty.Hard, new DifficultyConfig
            {
                CloudSpeed = 3.5f, BirdSpeed = 5f,
                SpawnInterval = 1.5f, MinSpawnInterval = 1f, MaxSpawnInterval = 3f
            }
        }
    };

    public static DifficultyConfig GetConfig(Difficulty difficulty)
    {
        return Configs.TryGetValue(difficulty, out var config) ? config : Configs[Difficulty.Easy];
    }
}
