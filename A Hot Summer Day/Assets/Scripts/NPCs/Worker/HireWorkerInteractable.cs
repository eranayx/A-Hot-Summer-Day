using System.Collections.Generic;
using UnityEngine;

public class HireWorkerInteractable : MonoBehaviour, IInteractable
{
    private Shop _shop;

    [SerializeField] private CustomerQueue _queue;
    [SerializeField] private List<Worker> _workerList;

    private void Awake()
    {
        _shop = GetComponentInParent<Shop>();
    }

    public string GetInteractText()
    {
        return $"Hire a worker to automate the process! [${_shop.GetWorkerHirePrice()}]";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact()
    {
        if (_shop.TryHireWorker())
        {
            Worker shopWorker = Instantiate(GetRandomWorker(), transform.position, Quaternion.identity);
            shopWorker.SetShop(_shop);
            shopWorker.SetQueue(_queue);

            Destroy(gameObject);
        }
    }

    public bool MetAllConditionsToInteract()
    {
        return true;
    }

    private Worker GetRandomWorker()
    {
        return _workerList[Random.Range(0, _workerList.Count)];
    }
}
