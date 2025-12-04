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
public class LevelManager : MonoBehaviour
{
    public static readonly bool IsShortcutsEnabled = false;
    
    public static Level currentLevel;

    private static MetricsManager _metricsManager;

    private void Start()
    {
        currentLevel = (Level)SceneManager.GetActiveScene().buildIndex;
        _metricsManager = FindObjectOfType<MetricsManager>();
    }

    private void Update()
    {
        if(!IsShortcutsEnabled) return;
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
        void ExportMetricsToCsv(string levelName)
        {
            var telemetryManager = FindObjectOfType<TelemetryManager>();
            if(telemetryManager)
                telemetryManager.ExportMetricsToCsv(levelName);
        }

        if (MetricsManager.IsTelemetryEnabled)
        {
            string levelName = SceneManager.GetActiveScene().name;
            _metricsManager?.RecordDateTimeNowFor(MetricsManager.DateTimeSampleType.GameEnd);
            ExportMetricsToCsv(levelName);
            _metricsManager?.ReinitializeMetrics();
        }
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
        IEnumerator LoadLevel(Level levelToLoad, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            var loadingScreenManagerGo = new GameObject("LoadingScreen");
            var loadingScreenManager = loadingScreenManagerGo.AddComponent<LoadingScreenManager>();
            loadingScreenManager.ShowLoadingScreen((int)levelToLoad);
            MealLauncher.shouldResetForNextLevel = true;
            currentLevel = levelToLoad;
            SceneManager.LoadScene((int)levelToLoad);
        }
        GameObject go = new GameObject("LevelManager");
        var levelManager = go.AddComponent<LevelManager>();
        levelManager.StartCoroutine(LoadLevel(level, delay));
    }

    public static void LoadLevelNow(Level levelToLoad)
    {
        LoadLevel(levelToLoad,0);
    }
}