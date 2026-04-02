using Unity.Cinemachine;
using UnityEngine;

public class ObstructionCollisionHandler : MonoBehaviour
{
    private CinemachineImpulseSource _impulseSource;
    private float _flashScale = 1f;
    private bool _hit;

    void Awake()
    {
        _impulseSource = GameObject.Find("Camera").GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float intensity = 1f)
    {
        _impulseSource.GenerateImpulse(intensity);
        if (DamageFeedback.Instance) DamageFeedback.Instance.Flash(intensity * _flashScale);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (_hit) return;

        if (collider.transform.CompareTag("Shield"))
        {
            _hit = true;
            Destroy(gameObject);
            AudioSourceManager.PlaySound(gameObject.name);
            return;
        }

        if (collider.transform.root.CompareTag("Player"))
        {
            _hit = true;
            Destroy(gameObject);
            AudioSourceManager.PlaySound(gameObject.name);
            Shake();
            GameEvents.FireObstructionHit(gameObject.name);
        }
    }
}
