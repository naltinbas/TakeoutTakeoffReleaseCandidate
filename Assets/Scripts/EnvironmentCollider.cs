using UnityEngine;

public class EnvironmentCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.transform.parent == null) return;
        GameObject parentGameObject = other.transform.parent.gameObject;
        if(parentGameObject == null) return;
        if (parentGameObject.CompareTag("Player"))
        {
            ExplosionHandler exHandler = parentGameObject.GetComponent<ExplosionHandler>();
            exHandler.Explode();
        }
    }
}
