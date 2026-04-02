using System;

// Observer pattern - static event bus for decoupling game systems.
public static class GameEvents
{
    public static event Action OnMealCollected;
    public static event Action OnMealDelivered;
    public static event Action OnExplosion;
    public static event Action OnFuelEmpty;
    public static event Action<int> OnDamageTaken;
    public static event Action<string> OnObstructionHit;
    public static event Action OnShieldPickup;
    public static event Action OnBoostPickup;
    public static event Action OnGameStart;
    public static event Action OnGamePause;
    public static event Action OnGameResume;
    public static event Action OnLevelComplete;
    public static event Action OnTutorialComplete;

    public static void FireMealCollected() => OnMealCollected?.Invoke();
    public static void FireMealDelivered() => OnMealDelivered?.Invoke();
    public static void FireExplosion() => OnExplosion?.Invoke();
    public static void FireFuelEmpty() => OnFuelEmpty?.Invoke();
    public static void FireDamageTaken(int amount) => OnDamageTaken?.Invoke(amount);
    public static void FireObstructionHit(string objectName) => OnObstructionHit?.Invoke(objectName);
    public static void FireShieldPickup() => OnShieldPickup?.Invoke();
    public static void FireBoostPickup() => OnBoostPickup?.Invoke();
    public static void FireGameStart() => OnGameStart?.Invoke();
    public static void FireGamePause() => OnGamePause?.Invoke();
    public static void FireGameResume() => OnGameResume?.Invoke();
    public static void FireLevelComplete() => OnLevelComplete?.Invoke();
    public static void FireTutorialComplete() => OnTutorialComplete?.Invoke();
}
