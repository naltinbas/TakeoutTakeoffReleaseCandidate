using UnityEngine;

// Singleton - centralizes shared meal state (was scattered as statics in MealLauncher).
public class MealTracker : Singleton<MealTracker>
{
    protected override bool Persistent => true;

    public int MaxMeals { get; private set; } = 4;
    public int NumLaunchedMeals { get; private set; }
    public int NumMealCollected { get; private set; }
    public int RemainingMeals { get; private set; }
    public bool IsLaunching { get; set; }
    public bool ShouldResetForNextLevel { get; set; }

    private bool _hasSetNumMeals;

    public void InitializeForLevel()
    {
        if (ShouldResetForNextLevel)
        {
            IsLaunching = false;
            NumMealCollected = 0;
            NumLaunchedMeals = 0;
            _hasSetNumMeals = false;
            ShouldResetForNextLevel = false;
        }

        if (!_hasSetNumMeals)
        {
            MaxMeals = GameObject.FindGameObjectsWithTag("Coin").Length;
            RemainingMeals = MaxMeals;
            _hasSetNumMeals = true;
        }
    }

    public void CollectMeal()
    {
        RemainingMeals--;
        NumMealCollected++;
    }

    public void LaunchMeal()
    {
        NumLaunchedMeals++;
    }

    public void ResetAll()
    {
        NumMealCollected = 0;
        NumLaunchedMeals = 0;
        RemainingMeals = MaxMeals;
        IsLaunching = false;
    }
}
