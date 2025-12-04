using System;
using UnityEngine;

public class CoinCollideHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        void DestroyCoin()
        {
            var tag = other.gameObject.tag;
            AudioSourceManager.PlaySound(tag);
            GetComponentInChildren<MeshRenderer>().enabled = false;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Destroy(gameObject);
        }

        void HandleTargetTelemetryFor(MetricsManager.DateTimeSampleType dateTimeSampleType)
        {
            if(!MetricsManager.IsTelemetryEnabled) return;
            var metricsManager = FindObjectOfType<MetricsManager>();
            if(!metricsManager) return;
            var dateTime = DateTime.Now;
            metricsManager.Record(dateTime, dateTimeSampleType);
        }
        
        MealLauncher mealLauncher = GetComponent<MealLauncher>();
        bool isCollected = mealLauncher.IsCollected;
        if (other.transform.root.CompareTag("Player") && !isCollected)
        {
            var playerTag = other.transform.parent.tag;
            AudioSourceManager.PlaySound(playerTag);
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            mealLauncher.Collect(gameObject);
            HandleTargetTelemetryFor(MetricsManager.DateTimeSampleType.HamburgerCollection);
            mealLauncher.UpdateMealCounterUI();
            if (MealLauncher.isLaunching) return;
            ObjectiveManager.SetObjectiveText("Deliver Meal into Target");
            ObjectiveManager.SetObjectiveColor(true);
        }

        if (other.CompareTag("Ground"))
        {
            DestroyCoin();
            HandleLevelCompletion();
           // FindObjectOfType<MealIconsUI>()?.OnMealDelivered();
        }

        if (other.CompareTag("Target"))
        {
            HandleTargetTelemetryFor(MetricsManager.DateTimeSampleType.HamburgerDelivery);
            // FindObjectOfType<MealIconsUI>()?.OnMealDelivered();
            DestroyCoin();
            HandleLevelCompletion();
        }
    }

    private void HandleLevelCompletion()
    {
        bool isThirdLevelCompleted = MealLauncher.numLaunchedMeals == MealLauncher.MaxMeals && 
                                     Level.FuturisticWorld == LevelManager.currentLevel, 
             isSecondLevelCompleted = MealLauncher.numLaunchedMeals == MealLauncher.MaxMeals && 
                                      Level.MedievalVillage == LevelManager.currentLevel,
             isFirstLevelCompleted = MealLauncher.numLaunchedMeals == MealLauncher.MaxMeals && 
                                     Level.FantasyVillage == LevelManager.currentLevel;
        if (isFirstLevelCompleted || isSecondLevelCompleted || isThirdLevelCompleted)
            LevelManager.ProceedToNextLevel();
    }
}