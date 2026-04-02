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

        MealLauncher mealLauncher = GetComponent<MealLauncher>();
        bool isCollected = mealLauncher.IsCollected;
        if (other.transform.root.CompareTag("Player") && !isCollected)
        {
            var playerTag = other.transform.parent.tag;
            AudioSourceManager.PlaySound(playerTag);
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            mealLauncher.Collect(gameObject);
            GameEvents.FireMealCollected();
            mealLauncher.UpdateMealCounterUI();
            if (MealTracker.Instance != null && MealTracker.Instance.IsLaunching) return;
            ObjectiveManager.SetObjectiveText("Deliver Meal into Target");
            ObjectiveManager.SetObjectiveColor(true);
        }

        if (other.CompareTag("Ground"))
        {
            DestroyCoin();
            HandleLevelCompletion();
        }

        if (other.CompareTag("Target"))
        {
            GameEvents.FireMealDelivered();
            DestroyCoin();
            HandleLevelCompletion();
        }
    }

    private void HandleLevelCompletion()
    {
        var tracker = MealTracker.Instance;
        if (tracker == null) return;

        bool allMealsLaunched = tracker.NumLaunchedMeals == tracker.MaxMeals;

        bool isThirdLevelCompleted = allMealsLaunched &&
                                     Level.FuturisticWorld == LevelManager.currentLevel;
        bool isSecondLevelCompleted = allMealsLaunched &&
                                      Level.MedievalVillage == LevelManager.currentLevel;
        bool isFirstLevelCompleted = allMealsLaunched &&
                                     Level.FantasyVillage == LevelManager.currentLevel;

        if (isFirstLevelCompleted || isSecondLevelCompleted || isThirdLevelCompleted)
            LevelManager.ProceedToNextLevel();
    }
}
