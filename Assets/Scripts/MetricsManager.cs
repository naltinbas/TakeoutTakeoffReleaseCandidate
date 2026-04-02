using System;
using UnityEngine;
using USCG.Core.Telemetry;

// Singleton - subscribes to GameEvents (Observer pattern) so other
// scripts dont need to find and call it directly.
public class MetricsManager : Singleton<MetricsManager>
{
    public static readonly bool IsTelemetryEnabled = false;

    private MetricId _explosionMetric = default,
        _hamburgerSuccessfulDeliveryTimes = default,
        _hamburgerCollectionTimes = default,
        _gameStartTime = default,
        _gameEndTime = default,
        _tutorialCompletionTime = default,
        _cloudCollisionsMetric = default,
        _birdCollisionsMetric = default;

    public enum AccumulationType
    {
        Explosion,
        BirdCollision,
        CloudCollision,
        None
    }

    public enum DateTimeSampleType
    {
        HamburgerDelivery,
        HamburgerCollection,
        TutorialCompletion,
        GameStart,
        GameEnd
    }

    void Start()
    {
        InitializeMetrics();
    }

    private void OnEnable()
    {
        GameEvents.OnExplosion += HandleExplosion;
        GameEvents.OnMealCollected += HandleMealCollected;
        GameEvents.OnMealDelivered += HandleMealDelivered;
        GameEvents.OnTutorialComplete += HandleTutorialComplete;
        GameEvents.OnObstructionHit += HandleObstructionHit;
    }

    private void OnDisable()
    {
        GameEvents.OnExplosion -= HandleExplosion;
        GameEvents.OnMealCollected -= HandleMealCollected;
        GameEvents.OnMealDelivered -= HandleMealDelivered;
        GameEvents.OnTutorialComplete -= HandleTutorialComplete;
        GameEvents.OnObstructionHit -= HandleObstructionHit;
    }

    private void HandleExplosion()
    {
        if (!IsTelemetryEnabled) return;
        Record(AccumulationType.Explosion);
        ClearTimeRecords(DateTimeSampleType.HamburgerDelivery);
        ClearTimeRecords(DateTimeSampleType.HamburgerCollection);
    }

    private void HandleMealCollected()
    {
        if (!IsTelemetryEnabled) return;
        Record(DateTime.Now, DateTimeSampleType.HamburgerCollection);
    }

    private void HandleMealDelivered()
    {
        if (!IsTelemetryEnabled) return;
        Record(DateTime.Now, DateTimeSampleType.HamburgerDelivery);
    }

    private void HandleTutorialComplete()
    {
        if (!IsTelemetryEnabled) return;
        Record(DateTime.Now, DateTimeSampleType.TutorialCompletion);
    }

    private void HandleObstructionHit(string objectName)
    {
        if (!IsTelemetryEnabled) return;
        AccumulationType accumulationType = objectName switch
        {
            "Birds" => AccumulationType.BirdCollision,
            "Cloud" => AccumulationType.CloudCollision,
            _ => AccumulationType.None
        };
        if (accumulationType == AccumulationType.None) return;
        Record(accumulationType);
    }

    private void InitializeMetrics()
    {
        _explosionMetric = TelemetryManager.instance.CreateAccumulatedMetric("Explosion Since");
        _hamburgerSuccessfulDeliveryTimes = TelemetryManager.instance.CreateSampledMetric<DateTime>("Successful Delivery Times Since Last Explosion");
        _hamburgerCollectionTimes = TelemetryManager.instance.CreateSampledMetric<DateTime>("Hamburger Collection Times Since Last Explosion");
        _gameStartTime = TelemetryManager.instance.CreateSampledMetric<DateTime>("Game Start Time");
        _gameEndTime = TelemetryManager.instance.CreateSampledMetric<DateTime>("Game End Time");
        _tutorialCompletionTime = TelemetryManager.instance.CreateSampledMetric<DateTime>("Tutorial Completion Time");
        _cloudCollisionsMetric = TelemetryManager.instance.CreateAccumulatedMetric("Cloud Collisions");
        _birdCollisionsMetric = TelemetryManager.instance.CreateAccumulatedMetric("Bird Collisions");
    }

    public void ReinitializeMetrics()
    {
        TelemetryManager.instance.ClearMetrics();
        InitializeMetrics();
    }

    private MetricId GetMetricId<T>(T sampleType)
    {
        MetricId metricId = default;
        switch (sampleType)
        {
            case DateTimeSampleType.HamburgerDelivery:
                metricId = _hamburgerSuccessfulDeliveryTimes;
                break;
            case DateTimeSampleType.HamburgerCollection:
                metricId = _hamburgerCollectionTimes;
                break;
            case DateTimeSampleType.TutorialCompletion:
                metricId = _tutorialCompletionTime;
                break;
            case DateTimeSampleType.GameStart:
                metricId = _gameStartTime;
                break;
            case DateTimeSampleType.GameEnd:
                metricId = _gameEndTime;
                break;
            case AccumulationType.Explosion:
                metricId = _explosionMetric;
                break;
            case AccumulationType.BirdCollision:
                metricId = _birdCollisionsMetric;
                break;
            case AccumulationType.CloudCollision:
                metricId = _cloudCollisionsMetric;
                break;
        }
        return metricId;
    }

    public void Record(AccumulationType accumulationType)
    {
        MetricId metricId = GetMetricId(accumulationType);
        TelemetryManager.instance.AccumulateMetric(metricId, 1);
    }

    public void Record(DateTime dateTime, DateTimeSampleType dateTimeSampleType)
    {
        MetricId metricId = GetMetricId(dateTimeSampleType);
        TelemetryManager.instance.AddMetricSample(metricId, dateTime);
    }

    public void ClearTimeRecords(DateTimeSampleType dateTimeSampleType)
    {
        MetricId metricId = GetMetricId(dateTimeSampleType);
        TelemetryManager.instance.ClearTimeSamples(metricId);
    }

    public void RecordDateTimeNowFor(DateTimeSampleType dateTimeSampleType)
    {
        Record(DateTime.Now, dateTimeSampleType);
    }
}
