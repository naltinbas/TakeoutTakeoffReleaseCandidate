using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaneBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float fuelIncreaseAmount = 10f;
    [SerializeField] private FuelSystem fuelSystem;

    [Header("UI")]
    [SerializeField] private Image boostIconPrefab; // Prefab of your Boost Icon (UI Image)
    [SerializeField] private RectTransform boostContainer; // Empty UI parent for icons
    [SerializeField] private float iconOffset = 30f; // How much each icon shifts (reduce for overlap)

    private AirplaneController controller;
    private bool isBoosting = false;
    private float originalSpeed;
    private List<Image> activeBoostIcons = new List<Image>();

    void Awake()
    {
        controller = GetComponent<AirplaneController>();

        // Hide any default icons if assigned directly
        if (boostIconPrefab != null)
            boostIconPrefab.gameObject.SetActive(false);
    }

    void Update()
    {
        // Trigger boost if available and not already active
        if (activeBoostIcons.Count > 0 && !isBoosting && Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(DoBoost());
        }
    }

    public void EnableBoost()
    {
        AudioSourceManager.PlaySound("GetBoost");

        // Add a new boost icon
        if (boostIconPrefab != null && boostContainer != null)
        {
            Image newIcon = Instantiate(boostIconPrefab, boostContainer);
            newIcon.gameObject.SetActive(true);

            // Slightly shift icons left for overlap
            float offsetX = -iconOffset * (activeBoostIcons.Count);
            newIcon.rectTransform.anchoredPosition = new Vector2(offsetX, 0);

            activeBoostIcons.Add(newIcon);
        }

        // Add fuel
        if (fuelSystem != null)
        {
            fuelSystem.AddFuel(fuelIncreaseAmount);
        }

        Debug.Log($"Boost collected! Total boosts: {activeBoostIcons.Count}");
    }

    private IEnumerator DoBoost()
    {
        if (activeBoostIcons.Count <= 0) yield break;

        isBoosting = true;
        AudioSourceManager.PlaySound("UseBoost");

        // Remove last icon visually
        Image lastIcon = activeBoostIcons[activeBoostIcons.Count - 1];
        activeBoostIcons.RemoveAt(activeBoostIcons.Count - 1);
        Destroy(lastIcon.gameObject);

        // Apply boost effect
        originalSpeed = controller.baseAirplaneSpeed;
        controller.baseAirplaneSpeed *= boostMultiplier;
        Debug.Log("Boost Activated!");

        yield return new WaitForSeconds(boostDuration);

        controller.baseAirplaneSpeed = originalSpeed;
        isBoosting = false;
        Debug.Log("Boost Ended!");
    }

    public void ResetBoosts()
    {
        // Clear all boost icons
        foreach (var icon in activeBoostIcons)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }

        activeBoostIcons.Clear();
        isBoosting = false;
    }
}
