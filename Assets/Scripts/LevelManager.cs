using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using USCG.Core.Telemetry;

public enum Level
{
    TitleMenu = 0,
    FantasyVillage = 1,
    MedievalVillage = 2,
    FuturisticWorld = 3
}

// Singleton - uses its own instance for coroutines instead of temporary GameObjects.
public class LevelManager : Singleton<LevelManager>
{
    public static readonly bool IsShortcutsEnabled = false;

    public static Level currentLevel;

    private void Start()
    {
        currentLevel = (Level)SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        if (!IsShortcutsEnabled) return;
        if (Input.GetKeyDown(KeyCode.M))
            LoadMainScreen();
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            LoadLevel(Level.FantasyVillage);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            LoadLevel(Level.MedievalVillage);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            LoadLevel(Level.FuturisticWorld);
    }

    public static void SetLevel(Level level)
    {
        currentLevel = level;
    }

    public static void ProceedToNextLevel()
    {
        if (MetricsManager.IsTelemetryEnabled)
        {
            string levelName = SceneManager.GetActiveScene().name;

            if (MetricsManager.Instance != null)
            {
                MetricsManager.Instance.RecordDateTimeNowFor(MetricsManager.DateTimeSampleType.GameEnd);
            }

            var telemetryManager = FindObjectOfType<TelemetryManager>();
            if (telemetryManager)
                telemetryManager.ExportMetricsToCsv(levelName);

            MetricsManager.Instance?.ReinitializeMetrics();
        }

        GameEvents.FireLevelComplete();
        LoadLevel(++currentLevel);
    }

    private void LoadMainScreen()
    {
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private static void LoadLevel(Level level, float delay = 0f)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(LoadLevelCoroutine(level, delay));
        }
        else
        {
            GameObject go = new GameObject("LevelManagerTemp");
            var tempManager = go.AddComponent<LevelManager>();
            tempManager.StartCoroutine(LoadLevelCoroutine(level, delay));
        }
    }

    private static IEnumerator LoadLevelCoroutine(Level levelToLoad, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        var loadingScreenManagerGo = new GameObject("LoadingScreen");
        var loadingScreenManager = loadingScreenManagerGo.AddComponent<LoadingScreenManager>();
        loadingScreenManager.ShowLoadingScreen((int)levelToLoad);
        MealLauncher.shouldResetForNextLevel = true;
        currentLevel = levelToLoad;
        SceneManager.LoadScene((int)levelToLoad);
    }

    public static void LoadLevelNow(Level levelToLoad)
    {
        LoadLevel(levelToLoad, 0);
    }
}
