using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button quitButton;

    private bool _hasPickedAnyLevel;
    private bool _hasRevealedDifficultySelector;
    private MetricsManager _metricsManager;

    public static Difficulty CurrentDifficulty { get; private set; }

    private void Start()
    {
        var pizzaCursor = Resources.Load<Texture2D>("cursorSmall");
        Cursor.SetCursor(pizzaCursor, new Vector2(8, 56), CursorMode.Auto);
        GameState.Current = GameStateType.Menu;
    }

    private void Awake()
    {
        ControlsScreen.StartGame += StartGame;
        SetupLevelButtons();
        SetupDifficultyButtons();
        backButton.onClick.AddListener(GoBack);
        controlsButton.onClick.AddListener(CreateRevealControlsAction());
        quitButton.onClick.AddListener(Application.Quit);
        _metricsManager = FindObjectOfType<MetricsManager>();
    }

    private void SetupLevelButtons()
    {
        level1Button.onClick.AddListener(() =>
        {
            SetLevel(Level.FantasyVillage);
            GoForward();
        });
        level2Button.onClick.AddListener(() =>
        {
            SetLevel(Level.MedievalVillage);
            GoForward();
        });
        level3Button.onClick.AddListener(() =>
        {
            SetLevel(Level.FuturisticWorld);
            GoForward();
        });
    }

    private void SetupDifficultyButtons()
    {
        easyButton.onClick.AddListener(() =>
        {
            CurrentDifficulty = Difficulty.Easy;
            HandleStartGame();
        });
        mediumButton.onClick.AddListener(() =>
        {
            CurrentDifficulty = Difficulty.Medium;
            HandleStartGame();
        });
        hardButton.onClick.AddListener(() =>
        {
            CurrentDifficulty = Difficulty.Hard;
            HandleStartGame();
        });
    }

    private void SetActiveDifficultySelectionButtons(bool isActive = true)
    {
        easyButton.gameObject.SetActive(isActive);
        mediumButton.gameObject.SetActive(isActive);
        hardButton.gameObject.SetActive(isActive);
    }

    private void SetActiveLevelSelectionButtons(bool isActive = true)
    {
        level1Button.gameObject.SetActive(isActive);
        level2Button.gameObject.SetActive(isActive);
        level3Button.gameObject.SetActive(isActive);
    }

    private void GoForward()
    {
        SetActiveLevelSelectionButtons(false);
        SetActiveDifficultySelectionButtons();
        backButton.gameObject.SetActive(true);
    }

    private void GoBack()
    {
        SetActiveDifficultySelectionButtons(false);
        SetActiveLevelSelectionButtons();
        backButton.gameObject.SetActive(false);
    }

    private void SetLevel(Level level)
    {
        LevelManager.SetLevel(level);
    }

    private void HandleStartGame()
    {
        if (ControlsScreen.hasAlreadyRevealed)
            StartGame();
        else
            CreateRevealControlsAction(true).Invoke();
    }

    private UnityAction CreateRevealControlsAction(bool shouldStartGame = false)
    {
        return () =>
        {
            ControlsScreen.hasAlreadyRevealed = false;
            var controlsScreen = gameObject.AddComponent<ControlsScreen>();
            controlsScreen.shouldStartGame = shouldStartGame;
        };
    }

    private void StartGame()
    {
        Time.timeScale = 1;
        GameState.Current = GameStateType.Playing;
        _metricsManager?.RecordDateTimeNowFor(MetricsManager.DateTimeSampleType.GameStart);
        GameEvents.FireGameStart();
        LevelManager.LoadLevelNow(LevelManager.currentLevel);
    }
}
