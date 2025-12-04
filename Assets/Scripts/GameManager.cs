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

    private bool hasPickedAnyLevel;

    private bool hasRevealedDifficultySelector;

    private MetricsManager _metricsManager;

    private void Start()
    {
        var pizzaCursor = Resources.Load<Texture2D>("cursorSmall");
        Cursor.SetCursor(pizzaCursor, new Vector2(8,56), CursorMode.Auto);
    }

    public static Difficulty CurrentDifficulty { get;private set; }
    private void Awake()
    {
        void SetActiveDifficultySelectionButtons(bool isActive = true)
        {
            easyButton.gameObject.SetActive(isActive);
            mediumButton.gameObject.SetActive(isActive);
            hardButton.gameObject.SetActive(isActive);
        }
        void SetActiveLevelSelectionButtons(bool isActive = true)
        {
            level1Button.gameObject.SetActive(isActive);
            level2Button.gameObject.SetActive(isActive);
            level3Button.gameObject.SetActive(isActive);
        }
        void GoForward()
        {
            SetActiveLevelSelectionButtons(false);
            SetActiveDifficultySelectionButtons();
            backButton.gameObject.SetActive(true);
        }
        void GoBack()
        {
            SetActiveDifficultySelectionButtons(false);
            SetActiveLevelSelectionButtons();
            backButton.gameObject.SetActive(false);
        }

        ControlsScreen.StartGame += StartGame;      
        level1Button.onClick.AddListener(()=>
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
        backButton.onClick.AddListener(GoBack);
        controlsButton.onClick.AddListener(RevealControls());
        quitButton.onClick.AddListener(Application.Quit);
        _metricsManager = FindObjectOfType<MetricsManager>();
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
            RevealControls(true).Invoke();
    }

    private UnityAction RevealControls(bool shouldStartGame = false)
    {
        void RevealControls(bool shouldStartGame = false)
        {
            ControlsScreen.hasAlreadyRevealed = false;
            var controlsScreen = gameObject.AddComponent<ControlsScreen>();
            controlsScreen.shouldStartGame = shouldStartGame;
        }
        return () =>
        {
            RevealControls(shouldStartGame);
        };
    }

    private void StartGame()
    {
        Time.timeScale = 1;
        _metricsManager?.RecordDateTimeNowFor(MetricsManager.DateTimeSampleType.GameStart);
        LevelManager.LoadLevelNow(LevelManager.currentLevel);
    }
}
