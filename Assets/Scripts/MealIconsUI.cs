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

    private List<Image> burgerIcons = new List<Image>();
    private int maxMeals;
    private int collectedMeals = 0;
    private int deliveredMeals = 0;

    private IEnumerator Start()
    {
        yield return null; // Wait for MealLauncher to initialize
        maxMeals = MealLauncher.MaxMeals;
        GenerateIcons();
        UpdateIconsUI();
        UpdateProgressBar(); // initialize progress bar
    }

    private void GenerateIcons()
    {
        foreach (Transform child in iconsParent)
            Destroy(child.gameObject);

        burgerIcons.Clear();

        for (int i = 0; i < maxMeals; i++)
        {
            Image newIcon = Instantiate(iconPrefab, iconsParent);
            newIcon.sprite = emptyBurgerSprite;
            newIcon.gameObject.SetActive(true);
            burgerIcons.Add(newIcon);
        }
    }

    // Called when burger (meal) is collected
    public void OnMealCollected()
    {
        if (collectedMeals < burgerIcons.Count)
        {
            burgerIcons[collectedMeals].sprite = filledBurgerSprite;
            collectedMeals++;
            // Progress bar NOT updated here (only updates on delivery)
        }
    }

    // Called when meal is successfully delivered
    public void OnMealDelivered()
    {
        if (deliveredMeals < collectedMeals)
        {
            burgerIcons[deliveredMeals].gameObject.SetActive(false);
            deliveredMeals++;
            UpdateProgressBar(); // Progress bar updates here
        }
    }

    public void ResetIcons()
    {
        collectedMeals = 0;
        deliveredMeals = 0;
        UpdateIconsUI();
        UpdateProgressBar();
    }

    private void UpdateIconsUI()
    {
        for (int i = 0; i < burgerIcons.Count; i++)
        {
            burgerIcons[i].gameObject.SetActive(true);
            burgerIcons[i].sprite = emptyBurgerSprite;
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBarFill != null && maxMeals > 0)
        {
            float fillPercent = (float)deliveredMeals / maxMeals;
            progressBarFill.fillAmount = fillPercent;
        }
    }
}
