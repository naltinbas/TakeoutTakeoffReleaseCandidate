using System;
using UnityEngine;
using USCG.Core.Telemetry;

public class MetricsManager : MonoBehaviour
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
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeMetrics();
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
