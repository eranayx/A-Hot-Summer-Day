using System.Collections.Generic;
using UnityEngine;

public class CustomerQueue : MonoBehaviour
{
    public Vector3 FirstQueuePosition { get; private set; }
    public bool IsCustomerPresent { get; private set; }
    
    private readonly List<Vector3> _waitingQueuePositions = new();
    private readonly List<Customer> _customersList = new();
    private Vector3 _queueEntrance;

    [SerializeField] private List<GameObject> _queuePositions;

    // Attempt to debug coffee shop customer rotation
    // [SerializeField] private Transform _cashierLocation;

    private void Start()
    {
        foreach (GameObject queuePosition in _queuePositions)
        {
            _waitingQueuePositions.Add(queuePosition.GetComponent<Transform>().position);
        }

        _queueEntrance = _waitingQueuePositions[_waitingQueuePositions.Count - 1];
        FirstQueuePosition = _waitingQueuePositions[0];
    }

    private void Update()
    {
        if (_customersList.Count > 0)
        {
            IsCustomerPresent = _customersList[0].transform.position == FirstQueuePosition;
        }
    }

    public bool CanAddCustomer()
    {
        return _customersList.Count < _waitingQueuePositions.Count;
    }

    public Customer Peek()
    {
        if (_customersList.Count == 0)
        {
            return null;
        }

        return _customersList[0];
    }

    // Enqueues a customer and moves them to their position in queue
    // ! Moving logic should be condensed into a function
    public void Enqueue(Customer customer)
    {
        _customersList.Add(customer);
        Vector3 waitingQueuePosition = _waitingQueuePositions[_customersList.IndexOf(customer)];

        NPC.RotationOptions rotationOption =
            waitingQueuePosition == FirstQueuePosition ? NPC.RotationOptions.RotateAfterMoving : NPC.RotationOptions.NoRotate;

        customer.MoveTo(_queueEntrance);
        customer.MoveTo(waitingQueuePosition, rotationOption);

        // Attempt to debug coffee shop customer rotation
        //if (waitingQueuePosition == FirstQueuePosition)
        //{
        //    customer.RotateTo(_cashierLocation.position);
        //}
    }

    // Dequeues the first customer and repositions everyone else accordingly
    public void Dequeue()
    {
        if (_customersList.Count == 0)
        {
            return;
        }

        _customersList[0].MoveToDespawn();
        _customersList.RemoveAt(0);

        for (int i = 0; i < _customersList.Count; i++)
        {
            NPC.RotationOptions rotationOption =
                _waitingQueuePositions[i] == FirstQueuePosition ? NPC.RotationOptions.RotateAfterMoving : NPC.RotationOptions.NoRotate;
            _customersList[i].MoveTo(_waitingQueuePositions[i], rotationOption);

            // Attempt to debug coffee shop customer rotation
            //if (_waitingQueuePositions[i] == FirstQueuePosition)
            //{
            //    _customersList[i].RotateTo(_cashierLocation.position);
            //}
        }
    }
}
