using UnityEngine;

public class BoostTrigger : MonoBehaviour
{
    private bool _collected;

    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;

        Transform root = other.transform.root;

        if (root.CompareTag("Player"))
        {
            _collected = true;

            PlaneBoost boost = root.GetComponent<PlaneBoost>();
            if (boost != null)
            {
                boost.EnableBoost();
            }

            Destroy(gameObject);
        }
    }
}
