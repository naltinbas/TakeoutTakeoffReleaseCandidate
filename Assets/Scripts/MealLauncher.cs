using System;
using System.Collections;
using UnityEngine;

public class MealLauncher : MonoBehaviour
{
    public static int MaxMeals = 4;
    private AirplaneController _airplaneController;
    private Transform _launchPoint;       // Where to spawn the ball

    private const float Gravity = -9.81f;
    private readonly Vector3 _initialVelocity = Vector3.up * Gravity;

    [Tooltip("Destroy the meal after this many seconds (0 = never).")]
    public float lifeTime = 20f;
    private float _coolDownTime = 1.5f;

    public static bool isLaunching;
    private static int _numMealCollected = 0;
    public static int numLaunchedMeals = 0;
    private static int _remainingMeals = MaxMeals;
    public static bool shouldResetForNextLevel;
    private static bool _hasSetNumMeals;

    private bool IsLaunched { get; set; }
    public bool IsCollected { get; private set; }

    private void Start()
    {
        // Reset state only when loading a new level
        if (shouldResetForNextLevel)
        {
            isLaunching = false;
            _numMealCollected = 0;
            numLaunchedMeals = 0;
            _hasSetNumMeals = false;
            shouldResetForNextLevel = false;
        }

        // Only update MaxMeals if it hasn't been set for this new level
        if (!_hasSetNumMeals)
        {
            _remainingMeals = MaxMeals = GameObject.FindGameObjectsWithTag("Coin").Length;
            _hasSetNumMeals = true;
        }

        var plane = GameObject.Find("Plane");
        if (!plane) return;

        _launchPoint = plane.transform;
        _airplaneController = plane.GetComponent<AirplaneController>();
    }

    public void Collect(GameObject mealGo)
    {
        if(gameObject == mealGo)
        {
            IsCollected = true;
            _remainingMeals--;
            _numMealCollected++;

            FindObjectOfType<MealIconsUI>()?.OnMealCollected();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsLaunched && IsCollected)
        {
            //FindObjectOfType<MealIconsUI>()?.OnMealDelivered();
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
        _numMealCollected = numLaunchedMeals = 0;
        _remainingMeals = MaxMeals;
        IsLaunched = false;
        IsCollected = false;
        isLaunching = false;
        _airplaneController.Reset();
        UpdateMealCounterUI();
        ObjectiveManager.SetObjectiveText("Collect Meal");
        ObjectiveManager.SetObjectiveColor(false);
        FindObjectOfType<MealIconsUI>()?.ResetIcons();
    }
    
    private IEnumerator ResetBallLaunched(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        isLaunching = false;
        ObjectiveManager.SetObjectiveText("Deliver Meal into Target");
        ObjectiveManager.SetObjectiveColor(true);
    }
    
    public void UpdateMealCounterUI()
    {
      //  ObjectiveManager.SetMealCounterText(_numMealCollected - numLaunchedMeals, _remainingMeals);
    }
    
    public void Launch()
    {
        if (gameObject == null || _launchPoint == null || IsLaunched || isLaunching)
        {
            Debug.LogWarning("Cannot deliver the meal either already dropped or null.");
            Debug.LogWarning(
    $"Cannot deliver meal: " +
    $"gameObjectNull={gameObject == null}, " +
    $"launchPointNull={_launchPoint == null}, " +
    $"isLaunched={IsLaunched}, " +
    $"isLaunching={isLaunching}");

            return;
        }

        isLaunching = true;

        // Create the meal
        GameObject meal = Instantiate(gameObject, _launchPoint.position, _launchPoint.rotation);

        // Ensure it has a Rigidbody
        Rigidbody rb = meal.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = meal.AddComponent<Rigidbody>();
        }
        MealLauncher mealLauncher = meal.GetComponent<MealLauncher>();
        IsLaunched = mealLauncher.IsLaunched = mealLauncher.IsCollected = true;
        numLaunchedMeals++;
        UpdateMealCounterUI();
        ObjectiveManager.SetObjectiveColor(false);
        ObjectiveManager.SetObjectiveText("Cooldown for relaunch");
        meal.GetComponentInChildren<MeshRenderer>().enabled = true;
        // Reset velocity in case prefab had something
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Apply initial velocity
        rb.linearVelocity = _launchPoint.TransformDirection(_initialVelocity);

        FindObjectOfType<MealIconsUI>()?.OnMealDelivered();

        // Optional lifetime
        if (lifeTime > 0f)
        {
            Destroy(meal, lifeTime);
            StartCoroutine(ResetBallLaunched(_coolDownTime));
        }
    }
}