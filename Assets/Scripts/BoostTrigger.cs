using UnityEngine;

public class BoostTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;

        // Option 1: Check by tag
        if (root.CompareTag("Player"))
        {
            PlaneBoost boost = root.GetComponent<PlaneBoost>();
            if (boost != null)
            {
                boost.EnableBoost();
            }

            Destroy(gameObject); // optional: remove trigger after one use
        }
    }
}
