using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class AirplaneController : MonoBehaviour
{
    [SerializeField] private PlaneHearts planeHearts;

    [Header("Propeller Reference")]
    public PropellerRotator propellerRotator;


    [Header("Fuel Reference")]
    public FuelSystem fuelSystem;

    [Header("Flight Settings")]
    public float baseAirplaneSpeed = 5f;
    public float descentSpeedWhenEmpty = 6f;
    public float glideSpeedMin = 1.5f;        // minimum forward glide speed
    public float glideDecayRate = 0.3f;       // how quickly the speed decays after fuel is gone

    private float currentSpeed;
    private bool _fuelEmpty = false;

    public bool IsGoingBack => (_yaw < 0 ? -_yaw : _yaw) % 360 > 90f
                               && (_yaw < 0 ? -_yaw : _yaw) % 360 < 270f;

    private bool _hasCompletedTutorial;

    private int _level
    {
        get
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Level1_SF": return 1;
                case "Level2_MedievalVillage": return 2;
                default: return -1;
            }
        }
    }

    private float _yaw = 0.0f,
        _pitch = 0.0f,
        _roll = 0.0f;

    private const float YawMultiplier = 120f,
        PitchMax = 20f,
        RollMax = 20f;

    public Transform lowerBound, upperBound;
    private GameObject _initialPositionGo;

    private void Start()
    {
        _initialPositionGo = GameObject.Find("InitialPosition");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = CalculateDifficultyBasedSpeed();

        // Auto-find FuelSystem if not assigned in Inspector
        if (fuelSystem == null)
            fuelSystem = FindObjectOfType<FuelSystem>();
    }

    private float CalculateDifficultyBasedSpeed()
    {
        return baseAirplaneSpeed * (int)LevelManager.currentLevel;
    }

    private void HandleTelemetryForTutorialCompletion()
    {
        if(!MetricsManager.IsTelemetryEnabled) return;
        var metricsManager = FindObjectOfType<MetricsManager>();
        if (!metricsManager) return;
        var dateTime = DateTime.Now;
        metricsManager.Record(dateTime, MetricsManager.DateTimeSampleType.TutorialCompletion);
    }

    void Update()
    {
        _hasCompletedTutorial = (_initialPositionGo &&
                                 _level == 1 &&
                                 !Mathf.Approximately(_initialPositionGo.transform.position.z, -20f)
                                 && transform.position.z > -20f) && !_hasCompletedTutorial;

        if (_hasCompletedTutorial)
        {
            _initialPositionGo.transform.position = new Vector3(17f,
                                                                _initialPositionGo.transform.position.y,
                                                                -20f);
            HandleTelemetryForTutorialCompletion();
        }

        if (RespawnIfNecessary()) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _yaw += horizontal * YawMultiplier * Time.deltaTime;
        _pitch = Mathf.Lerp(0f, PitchMax, Mathf.Abs(vertical)) * Mathf.Sign(-vertical);
        _roll = Mathf.Lerp(0f, RollMax, Mathf.Abs(horizontal)) * Mathf.Sign(-horizontal);

        transform.localRotation = Quaternion.Euler(Vector3.up * _yaw +
                                                   Vector3.right * _pitch +
                                                   Vector3.forward * _roll);

        // Smooth speed decay if fuel is empty
        if (_fuelEmpty)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, glideSpeedMin, Time.deltaTime * glideDecayRate);
        }
        else
        {
            currentSpeed = CalculateDifficultyBasedSpeed();
        }

        // Movement behavior
        if (!_fuelEmpty)
        {
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
        }
        else
        {
            // Gradual glide + descent
            Vector3 glideDirection = (transform.forward + Vector3.down * 0.4f).normalized;
            transform.position += glideDirection * currentSpeed * Time.deltaTime;
            transform.position += Vector3.down * (descentSpeedWhenEmpty * 0.3f) * Time.deltaTime;

            // Smooth nose dip
            Quaternion targetRotation = Quaternion.Euler(25f, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.5f);
        }
    }

    private bool RespawnIfNecessary()
    {
        bool shouldReset = !IsInMissionArea(lowerBound.position, upperBound.position);
        if (shouldReset)
        {
            ExplosionHandler explosionHandler = GetComponent<ExplosionHandler>();
            if (!explosionHandler) return true;
            explosionHandler.Explode();
        }

        return shouldReset;
    }

    private bool IsInMissionArea(Vector3 lowerCorner, Vector3 upperCorner)
    {
        var pos = transform.position;
        return pos.x > lowerCorner.x && pos.x < upperCorner.x &&
               pos.z > lowerCorner.z && pos.z < upperCorner.z;
    }

    public void Reset()
    {
        transform.position = GameObject.Find("InitialPosition").transform.position;
        _yaw = 0f;
        _pitch = 0f;
        _roll = 0f;

        if (planeHearts != null)
            planeHearts.ResetHearts();

        if (fuelSystem != null)
            fuelSystem.ResetFuel();

        PlaneBoost planeBoost = GetComponent<PlaneBoost>();
        if (planeBoost != null)
            planeBoost.ResetBoosts();

        _fuelEmpty = false;
        currentSpeed = CalculateDifficultyBasedSpeed();

        if (propellerRotator != null)
            propellerRotator.ResumeRotation();

        AudioSourceManager.PlayPersistentAudio();

        //SpawnManager sm = FindObjectOfType<SpawnManager>();
        //if (sm != null)
        //    sm.ResetPickups();

        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();

        spawnManager.ResetPickups();


    }

    public void OnFuelEmpty()
    {
        if (propellerRotator != null)
            propellerRotator.StopRotation();

        AudioSourceManager.PauseAudio();

        if (_fuelEmpty) return;
        _fuelEmpty = true;
    }
}
