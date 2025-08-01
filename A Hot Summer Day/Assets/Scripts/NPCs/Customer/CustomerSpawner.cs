using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Customer Details")]
    [SerializeField] private CustomerDespawner _despawner;
    [SerializeField] private CustomerQueue _queue;

    [Header("Customer List")]
    [SerializeField] private List<Customer> _customerList;

    private Shop _shop;
    private float _spawnCooldown = 5;
    private float _timer;

    private void Awake()
    {
        _shop = GetComponentInParent<Shop>();    
    }

    private void Update()
    {
        if (_timer < _spawnCooldown)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _timer = 0;
            TryQueueCustomer();
            _spawnCooldown = RandomizeSpawnCooldown();
        }
    }

    private void TryQueueCustomer()
    {
        if (_queue.CanAddCustomer())
        {
            Customer customer = Instantiate(GetRandomCustomer(), transform.position, transform.rotation);
            customer.SetShop(_shop);
            customer.SetDespawner(_despawner);
            customer.SetQueue(_queue);

            _queue.Enqueue(customer);
        }
    }

    private float RandomizeSpawnCooldown()
    {
        float minimumCooldown = 4f;
        float maximumCooldown = 8f;

        return Random.Range(minimumCooldown, maximumCooldown);
    }

    private Customer GetRandomCustomer()
    {
        return _customerList[Random.Range(0, _customerList.Count)];
    }
}
