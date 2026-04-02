using System;
using System.Collections;
using UnityEngine;

public class MealLauncher : MonoBehaviour
{
    // Static proxies for backwards compatibility - delegate to MealTracker singleton
    public static int MaxMeals => MealTracker.Instance != null ? MealTracker.Instance.MaxMeals : 4;
    public static int numLaunchedMeals => MealTracker.Instance != null ? MealTracker.Instance.NumLaunchedMeals : 0;
    public static bool isLaunching
    {
        get => MealTracker.Instance != null && MealTracker.Instance.IsLaunching;
        set { if (MealTracker.Instance != null) MealTracker.Instance.IsLaunching = value; }
    }
    public static bool shouldResetForNextLevel
    {
        get => MealTracker.Instance != null && MealTracker.Instance.ShouldResetForNextLevel;
        set { if (MealTracker.Instance != null) MealTracker.Instance.ShouldResetForNextLevel = value; }
    }

    private AirplaneController _airplaneController;
    private Transform _launchPoint;

    private const float Gravity = -9.81f;
    private readonly Vector3 _initialVelocity = Vector3.up * Gravity;

    [Tooltip("Destroy the meal after this many seconds (0 = never).")]
    public float lifeTime = 20f;
    private float _coolDownTime = 1.5f;

    private bool IsLaunched { get; set; }
    public bool IsCollected { get; private set; }

    private void Start()
    {
        if (MealTracker.Instance == null)
        {
            var go = new GameObject("MealTracker");
            go.AddComponent<MealTracker>();
        }

        MealTracker.Instance.InitializeForLevel();

        var plane = GameObject.Find("Plane");
        if (!plane) return;

        _launchPoint = plane.transform;
        _airplaneController = plane.GetComponent<AirplaneController>();
    }

    public void Collect(GameObject mealGo)
    {
        if (gameObject == mealGo)
        {
            IsCollected = true;
            MealTracker.Instance.CollectMeal();
            FindObjectOfType<MealIconsUI>()?.OnMealCollected();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsLaunched && IsCollected)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            Launch();
        }
        else if (Input.GetKeyDown(KeyCode.R) && LevelManager.IsShortcutsEnabled)
        {
            ResetState();
        }
    }

    public void ResetState()
    {
        if (gameObject.CompareTag("Coin"))
        {
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
        }

        MealTracker.Instance.ResetAll();
        IsLaunched = false;
        IsCollected = false;
        _airplaneController.Reset();
        UpdateMealCounterUI();
        ObjectiveManager.SetObjectiveText("Collect Meal");
        ObjectiveManager.SetObjectiveColor(false);
        FindObjectOfType<MealIconsUI>()?.ResetIcons();
    }

    private IEnumerator ResetBallLaunched(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        MealTracker.Instance.IsLaunching = false;
        ObjectiveManager.SetObjectiveText("Deliver Meal into Target");
        ObjectiveManager.SetObjectiveColor(true);
    }

    public void UpdateMealCounterUI()
    {
    }

    public void Launch()
    {
        var tracker = MealTracker.Instance;

        if (gameObject == null || _launchPoint == null || IsLaunched || tracker.IsLaunching)
        {
            Debug.LogWarning(
                $"Cannot deliver meal: " +
                $"gameObjectNull={gameObject == null}, " +
                $"launchPointNull={_launchPoint == null}, " +
                $"isLaunched={IsLaunched}, " +
                $"isLaunching={tracker.IsLaunching}");
            return;
        }

        tracker.IsLaunching = true;

        GameObject meal = Instantiate(gameObject, _launchPoint.position, _launchPoint.rotation);

        Rigidbody rb = meal.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = meal.AddComponent<Rigidbody>();
        }

        MealLauncher mealLauncher = meal.GetComponent<MealLauncher>();
        IsLaunched = mealLauncher.IsLaunched = mealLauncher.IsCollected = true;

        tracker.LaunchMeal();
        UpdateMealCounterUI();

        ObjectiveManager.SetObjectiveColor(false);
        ObjectiveManager.SetObjectiveText("Cooldown for relaunch");
        meal.GetComponentInChildren<MeshRenderer>().enabled = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = _launchPoint.TransformDirection(_initialVelocity);

        FindObjectOfType<MealIconsUI>()?.OnMealDelivered();

        if (lifeTime > 0f)
        {
            Destroy(meal, lifeTime);
            StartCoroutine(ResetBallLaunched(_coolDownTime));
        }
    }
}
