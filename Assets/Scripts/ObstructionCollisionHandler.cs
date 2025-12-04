using Unity.Cinemachine;
using UnityEngine;

public class ObstructionCollisionHandler : MonoBehaviour
{
    
    private CinemachineImpulseSource impulseSource;
    private float flashScale = 1f;

    void Awake()
    {
        impulseSource = GameObject.Find("Camera").GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float intensity = 1f)
    {
        impulseSource.GenerateImpulse(intensity);
        if (DamageFeedback.Instance) DamageFeedback.Instance.Flash(intensity * flashScale);
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.CompareTag("Shield"))
        {
            Destroy(gameObject);
            AudioSourceManager.PlaySound(gameObject.name);
            return;
        }
        
        if (collider.transform.root.CompareTag("Player"))
        {
            Destroy(gameObject);
            AudioSourceManager.PlaySound(gameObject.name);
            Shake();
            RecordCollsion(gameObject.name);
        }
    }

    private void RecordCollsion(string collidedObjectName)
    {
        if(!MetricsManager.IsTelemetryEnabled) return;
        var metricsManager = FindObjectOfType<MetricsManager>();
        if(!metricsManager) return;
        MetricsManager.AccumulationType accumulationType = MetricsManager.AccumulationType.None;
        switch (collidedObjectName)
        {
            case "Birds":
                accumulationType = MetricsManager.AccumulationType.BirdCollision;
                break;
            case "Cloud":
                accumulationType = MetricsManager.AccumulationType.CloudCollision;
                break;
        }
        if(accumulationType == MetricsManager.AccumulationType.None) return;
        metricsManager.Record(accumulationType);
    }
}
