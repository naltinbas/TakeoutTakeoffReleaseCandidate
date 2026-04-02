using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private bool _hit;

    private void OnTriggerEnter(Collider other)
    {
        if (_hit) return;

        if (other.transform.root.CompareTag("Player"))
        {
            _hit = true;

            PlaneHearts planeHearts = other.transform.root.GetComponent<PlaneHearts>();
            if (planeHearts != null)
            {
                planeHearts.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
