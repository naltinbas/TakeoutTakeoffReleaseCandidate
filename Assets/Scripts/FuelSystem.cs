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
    private Vector3 baseScale;

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

    private float currentFuel;
    private bool isLowFuel = false;
    private bool isEmpty = false;
    private Coroutine heartbeatRoutine;
    private Coroutine glowRoutine;

    void Start()
    {
        baseScale = fuelPanelToPulse.localScale;
        currentFuel = maxFuel;
        UpdateUI();
    }

    void Update()
    {
        if (isEmpty) return;

        // Consume fuel over time
        currentFuel -= consumptionRate * Time.deltaTime;
        currentFuel = Mathf.Max(0, currentFuel);

        float fuelPercent = currentFuel / maxFuel;
        fuelFill.fillAmount = fuelPercent;

        // Full (green)
        if (fuelPercent > lowFuelThreshold)
        {
            fuelFill.color = fullFuelColor;
        }
        // Low (yellow + gentle pulse)
        else if (fuelPercent > 0 && !isEmpty)
        {
            fuelFill.color = lowFuelColor;

            if (!isLowFuel)
            {
                isLowFuel = true;
                StartHeartbeat(3.5f, 0.1f);
            }
        }

        // Stop heartbeat if refueled
        if (isLowFuel && fuelPercent > lowFuelThreshold)
        {
            isLowFuel = false;
            StopHeartbeat();
        }

        // Empty (red fill, no pulse)
        if (!isEmpty && currentFuel <= 0)
        {
            isEmpty = true;
            isLowFuel = false;

            StopHeartbeat(); // Stop any existing pulse
            fuelFill.fillAmount = 1f; // Fill completely red
            fuelFill.color = emptyFuelColor;
            fuelLabel.text = "Fuel Empty";

            airplaneController.OnFuelEmpty();
        }
        else if (!isEmpty)
        {
            fuelLabel.text = "Fuel";
        }
    }

    void StartHeartbeat(float speed, float magnitude)
    {
        if (heartbeatRoutine == null)
            heartbeatRoutine = StartCoroutine(Heartbeat(speed, magnitude));
    }

    void StopHeartbeat()
    {
        if (heartbeatRoutine != null)
        {
            StopCoroutine(heartbeatRoutine);
            heartbeatRoutine = null;
        }
        fuelPanelToPulse.localScale = baseScale;
    }

    IEnumerator Heartbeat(float speed, float magnitude)
    {
        Vector3 baseScale = fuelPanelToPulse.localScale;

        while (true)
        {
            float t = Mathf.Sin(Time.time * speed * Mathf.PI) * 0.5f + 0.5f;
            fuelPanelToPulse.localScale = baseScale * (1f + t * magnitude);
            yield return null;
        }
    }

    void UpdateUI()
    {
        fuelFill.fillAmount = currentFuel / maxFuel;
        fuelFill.color = fullFuelColor;
        fuelLabel.text = "Fuel";
    }

    public void AddFuel(float amount)
    {
        currentFuel = Mathf.Min(currentFuel + amount, maxFuel);
        UpdateUI();
        PlayFuelGlow(); // Trigger glow when fuel increases
    }

    // Glow Effect Coroutine
    public void PlayFuelGlow()
    {
        if (glowRoutine != null)
            StopCoroutine(glowRoutine);

        glowRoutine = StartCoroutine(FuelGlowEffect());
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
        glowRoutine = null;
    }

    public void ResetFuel()
    {
        currentFuel = maxFuel;
        isEmpty = false;
        isLowFuel = false;
        StopHeartbeat();
        UpdateUI();
    }
}
