using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelSystem : MonoBehaviour
{
    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float consumptionRate = 5f; // units per second
    public float lowFuelThreshold = 0.25f; // 25% triggers yellow pulse
    private Vector3 _baseScale;

    [Header("UI")]
    public Image fuelFill;
    public TMP_Text fuelLabel;
    public RectTransform fuelPanelToPulse;

    [Header("Colors")]
    public Color fullFuelColor = Color.green;
    public Color lowFuelColor = Color.yellow;
    public Color emptyFuelColor = Color.red;

    [Header("Glow Effect")]
    [SerializeField] private float fuelGlowDuration = 0.5f; // how long the glow lasts
    [SerializeField] private float fuelGlowIntensity = 2f;  // how strong the glow is

    [Header("Plane Reference")]
    public AirplaneController airplaneController;

    private float _currentFuel;
    private bool _isLowFuel = false;
    private bool _isEmpty = false;
    private Coroutine _heartbeatRoutine;
    private Coroutine _glowRoutine;

    void Start()
    {
        _baseScale = fuelPanelToPulse.localScale;
        _currentFuel = maxFuel;
        UpdateUI();
    }

    void Update()
    {
        if (_isEmpty) return;

        // Consume fuel over time
        _currentFuel -= consumptionRate * Time.deltaTime;
        _currentFuel = Mathf.Max(0, _currentFuel);

        float fuelPercent = _currentFuel / maxFuel;
        fuelFill.fillAmount = fuelPercent;

        // Full (green)
        if (fuelPercent > lowFuelThreshold)
        {
            fuelFill.color = fullFuelColor;
        }
        // Low (yellow + gentle pulse)
        else if (fuelPercent > 0 && !_isEmpty)
        {
            fuelFill.color = lowFuelColor;

            if (!_isLowFuel)
            {
                _isLowFuel = true;
                StartHeartbeat(3.5f, 0.1f);
            }
        }

        // Stop heartbeat if refueled
        if (_isLowFuel && fuelPercent > lowFuelThreshold)
        {
            _isLowFuel = false;
            StopHeartbeat();
        }

        // Empty (red fill, no pulse)
        if (!_isEmpty && _currentFuel <= 0)
        {
            _isEmpty = true;
            _isLowFuel = false;

            StopHeartbeat(); // Stop any existing pulse
            fuelFill.fillAmount = 1f; // Fill completely red
            fuelFill.color = emptyFuelColor;
            fuelLabel.text = "Fuel Empty";

            airplaneController.OnFuelEmpty();
        }
        else if (!_isEmpty)
        {
            fuelLabel.text = "Fuel";
        }
    }

    void StartHeartbeat(float speed, float magnitude)
    {
        if (_heartbeatRoutine == null)
            _heartbeatRoutine = StartCoroutine(Heartbeat(speed, magnitude));
    }

    void StopHeartbeat()
    {
        if (_heartbeatRoutine != null)
        {
            StopCoroutine(_heartbeatRoutine);
            _heartbeatRoutine = null;
        }
        fuelPanelToPulse.localScale = _baseScale;
    }

    IEnumerator Heartbeat(float speed, float magnitude)
    {
        Vector3 _baseScale = fuelPanelToPulse.localScale;

        while (true)
        {
            float t = Mathf.Sin(Time.time * speed * Mathf.PI) * 0.5f + 0.5f;
            fuelPanelToPulse.localScale = _baseScale * (1f + t * magnitude);
            yield return null;
        }
    }

    void UpdateUI()
    {
        fuelFill.fillAmount = _currentFuel / maxFuel;
        fuelFill.color = fullFuelColor;
        fuelLabel.text = "Fuel";
    }

    public void AddFuel(float amount)
    {
        _currentFuel = Mathf.Min(_currentFuel + amount, maxFuel);
        UpdateUI();
        PlayFuelGlow(); // Trigger glow when fuel increases
    }

    // Glow Effect Coroutine
    public void PlayFuelGlow()
    {
        if (_glowRoutine != null)
            StopCoroutine(_glowRoutine);

        _glowRoutine = StartCoroutine(FuelGlowEffect());
    }

    private IEnumerator FuelGlowEffect()
    {
        Color baseColor = fuelFill.color;
        float timer = 0f;

        while (timer < fuelGlowDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Sin((timer / fuelGlowDuration) * Mathf.PI); // smooth in-out
            fuelFill.color = Color.Lerp(baseColor, Color.white, t * fuelGlowIntensity * 0.5f);
            yield return null;
        }

        fuelFill.color = baseColor;
        _glowRoutine = null;
    }

    public void ResetFuel()
    {
        _currentFuel = maxFuel;
        _isEmpty = false;
        _isLowFuel = false;
        StopHeartbeat();
        UpdateUI();
    }
}
