using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MealIconsUI : MonoBehaviour
{
    [Header("Burger Icon Settings")]
    [SerializeField] private Transform iconsParent;
    [SerializeField] private Image iconPrefab;
    [SerializeField] private Sprite emptyBurgerSprite;
    [SerializeField] private Sprite filledBurgerSprite;

    [Header("Progress Bar Settings")]
    [SerializeField] private Image progressBarFill; // Assign your ProgressBarFill image here

    private List<Image> _burgerIcons = new List<Image>();
    private int _maxMeals;
    private int _collectedMeals = 0;
    private int _deliveredMeals = 0;

    private IEnumerator Start()
    {
        yield return null; // Wait for MealLauncher to initialize
        _maxMeals = MealLauncher.MaxMeals;
        GenerateIcons();
        UpdateIconsUI();
        UpdateProgressBar(); // initialize progress bar
    }

    private void GenerateIcons()
    {
        foreach (Transform child in iconsParent)
            Destroy(child.gameObject);

        _burgerIcons.Clear();

        for (int i = 0; i < _maxMeals; i++)
        {
            Image newIcon = Instantiate(iconPrefab, iconsParent);
            newIcon.sprite = emptyBurgerSprite;
            newIcon.gameObject.SetActive(true);
            _burgerIcons.Add(newIcon);
        }
    }

    // Called when burger (meal) is collected
    public void OnMealCollected()
    {
        if (_collectedMeals < _burgerIcons.Count)
        {
            _burgerIcons[_collectedMeals].sprite = filledBurgerSprite;
            _collectedMeals++;
            // Progress bar NOT updated here (only updates on delivery)
        }
    }

    // Called when meal is successfully delivered
    public void OnMealDelivered()
    {
        if (_deliveredMeals < _collectedMeals)
        {
            _burgerIcons[_deliveredMeals].gameObject.SetActive(false);
            _deliveredMeals++;
            UpdateProgressBar(); // Progress bar updates here
        }
    }

    public void ResetIcons()
    {
        _collectedMeals = 0;
        _deliveredMeals = 0;
        UpdateIconsUI();
        UpdateProgressBar();
    }

    private void UpdateIconsUI()
    {
        for (int i = 0; i < _burgerIcons.Count; i++)
        {
            _burgerIcons[i].gameObject.SetActive(true);
            _burgerIcons[i].sprite = emptyBurgerSprite;
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBarFill != null && _maxMeals > 0)
        {
            float fillPercent = (float)_deliveredMeals / _maxMeals;
            progressBarFill.fillAmount = fillPercent;
        }
    }
}
