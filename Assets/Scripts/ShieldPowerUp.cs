using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    [SerializeField] private float shieldDuration = 3f; // customizable duration

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player hit the power-up
        PlaneHearts planeHearts = other.transform.root.GetComponent<PlaneHearts>();
        if (planeHearts != null)
        {
            planeHearts.PickupShield();      // Enable shield icon
            AudioSourceManager.PlaySound("GetShield");
            Destroy(gameObject);             // Remove the power-up from the scene
        }
    }
}
