using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{

    [SerializeField] private PlaneHearts planeHearts;

    public void Explode()
    {
        ResetLevelState();
        AudioSourceManager.PlaySound("crash");
        HandleTelemetry();
    }

    public void ResetLevelState()
    {
        MealLauncher[] mealLaunchers = FindObjectsOfType<MealLauncher>();
        foreach (var mealLauncher in mealLaunchers)
        {
            mealLauncher.ResetState();
        }
        /// Reset hearts to full
        planeHearts?.ResetHearts();
    }

    private void HandleTelemetry()
    {
        if(!MetricsManager.IsTelemetryEnabled) return;
        var metricsManager = FindObjectOfType<MetricsManager>();
        if(!metricsManager) return;
        metricsManager.Record(MetricsManager.AccumulationType.Explosion);
        metricsManager.ClearTimeRecords(MetricsManager.DateTimeSampleType.HamburgerDelivery);
        metricsManager.ClearTimeRecords(MetricsManager.DateTimeSampleType.HamburgerCollection);
    }
}
