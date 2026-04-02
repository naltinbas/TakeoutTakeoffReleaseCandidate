using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    [SerializeField] private float shieldDuration = 3f;

    private bool _collected;

    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;

        PlaneHearts planeHearts = other.transform.root.GetComponent<PlaneHearts>();
        if (planeHearts != null)
        {
            _collected = true;

            planeHearts.PickupShield();
            AudioSourceManager.PlaySound("GetShield");
            Destroy(gameObject);
        }
    }
}
