using UnityEngine;

public class CustomerDespawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Customer customer))
        {
            Destroy(customer.gameObject);
        }
    }
}
