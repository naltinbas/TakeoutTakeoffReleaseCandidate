using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    [SerializeField] private PlaneHearts planeHearts;

    public void Explode()
    {
        ResetLevelState();
        AudioSourceManager.PlaySound("crash");
        GameEvents.FireExplosion();
    }

    public void ResetLevelState()
    {
        MealLauncher[] mealLaunchers = FindObjectsOfType<MealLauncher>();
        foreach (var mealLauncher in mealLaunchers)
        {
            mealLauncher.ResetState();
        }

        planeHearts?.ResetHearts();
    }
}
