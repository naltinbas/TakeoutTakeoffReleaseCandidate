using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            PlaneHearts planeHearts = other.transform.root.GetComponent<PlaneHearts>();
            if (planeHearts != null)
            {
                // Apply heart-based damage
                planeHearts.TakeDamage(damage);
            }

            // Optional: destroy the obstacle on hit
            Destroy(gameObject);
        }
    }
}
