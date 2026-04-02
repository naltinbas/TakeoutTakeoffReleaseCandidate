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

    private AirplaneController _controller;
    private bool _isBoosting = false;
    private float _originalSpeed;
    private List<Image> _activeBoostIcons = new List<Image>();

    void Awake()
    {
        _controller = GetComponent<AirplaneController>();

        // Hide any default icons if assigned directly
        if (boostIconPrefab != null)
            boostIconPrefab.gameObject.SetActive(false);
    }

    void Update()
    {
        // Trigger boost if available and not already active
        if (_activeBoostIcons.Count > 0 && !_isBoosting && Input.GetKeyDown(KeyCode.K))
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
            float offsetX = -iconOffset * (_activeBoostIcons.Count);
            newIcon.rectTransform.anchoredPosition = new Vector2(offsetX, 0);

            _activeBoostIcons.Add(newIcon);
        }

        // Add fuel
        if (fuelSystem != null)
        {
            fuelSystem.AddFuel(fuelIncreaseAmount);
        }

        Debug.Log($"Boost collected! Total boosts: {_activeBoostIcons.Count}");
    }

    private IEnumerator DoBoost()
    {
        if (_activeBoostIcons.Count <= 0) yield break;

        _isBoosting = true;
        AudioSourceManager.PlaySound("UseBoost");

        // Remove last icon visually
        Image lastIcon = _activeBoostIcons[_activeBoostIcons.Count - 1];
        _activeBoostIcons.RemoveAt(_activeBoostIcons.Count - 1);
        Destroy(lastIcon.gameObject);

        // Apply boost effect
        _originalSpeed = _controller.baseAirplaneSpeed;
        _controller.baseAirplaneSpeed *= boostMultiplier;
        Debug.Log("Boost Activated!");

        yield return new WaitForSeconds(boostDuration);

        _controller.baseAirplaneSpeed = _originalSpeed;
        _isBoosting = false;
        Debug.Log("Boost Ended!");
    }

    public void ResetBoosts()
    {
        // Clear all boost icons
        foreach (var icon in _activeBoostIcons)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }

        _activeBoostIcons.Clear();
        _isBoosting = false;
    }
}
