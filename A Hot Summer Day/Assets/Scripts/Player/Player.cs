using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    public class OnMoneyChangedEventArgs : EventArgs
    {
        public float moneyToAdd;
        public bool recievedTip = false;
        public bool isNotification = false;
    }

    public event EventHandler<OnMoneyChangedEventArgs> OnMoneyChanged;
    public event EventHandler OnTimedInteractableInteraction;

    public bool isNotificationsEnabled = true;
    public bool isMovementSuspended = false;
    public bool isInputtingText = false;

    public float Money { get; private set; } = 0;
    public float Speed { get; private set; } = 7f;
    public bool IsHoldingItem { get; private set; } = false;
    public bool InteractingWithUI { get; private set; } = false;

    private FirstPersonController _firstPersonController;

    [SerializeField] private GameObject _heldItem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        _firstPersonController = GetComponent<FirstPersonController>();
    }

    private void Start()
    {
        LockCursor();
    }

    // Check for nearby interactables
    private void Update()
    {
        if (!isInputtingText && Input.GetKeyDown(KeyCode.E))
        {
            IInteractable closestInteractable = GetClosestInteractable();
            
            if (closestInteractable != null)
            {
                TryInteractWith(closestInteractable);
            }
        }
    }

    // Returns the closest interactable within a certain range, if any,
    // using overlapSphere and colliders
    // Credits: https://www.youtube.com/watch?v=5ABYVYuOK-A
    public IInteractable GetClosestInteractable()
    {
        float interactRange = 3f;
        List<IInteractable> interactableList = new();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                interactableList.Add(interactable);
            }
        }

        IInteractable closestInteractable = null;

        foreach (IInteractable interactable in interactableList)
        {
            closestInteractable ??= interactable;

            if (Vector3.Distance(transform.position, interactable.GetTransform().position)
                < Vector3.Distance(transform.position, closestInteractable.GetTransform().position))
            {
                closestInteractable = interactable;
            }
        }

        return closestInteractable;
    }

    public void AddMoney(float moneyToAdd, bool recievedTip=false, bool isNotification=false)
    {
        Money += moneyToAdd;

        OnMoneyChanged?.Invoke(this, new OnMoneyChangedEventArgs
        {
            moneyToAdd = moneyToAdd,
            recievedTip = recievedTip,
            isNotification = isNotification
        });
    }

    public void HoldItem(GameObject item)
    {
        _heldItem.GetComponent<MeshFilter>().sharedMesh = item.GetComponent<MeshFilter>().sharedMesh;
        _heldItem.GetComponent<MeshRenderer>().sharedMaterial = item.GetComponent<MeshRenderer>().sharedMaterial;
        IsHoldingItem = true;
    }

    public void ReleaseItem()
    {
        _heldItem.GetComponent<MeshFilter>().sharedMesh = null;
        _heldItem.GetComponent<MeshRenderer>().sharedMaterial = null;
        IsHoldingItem = false;
    }

    public void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InteractingWithUI = false;
    }

    public void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        InteractingWithUI = true;
    }

    public void SetSensitivity(float sensitivity)
    {
        _firstPersonController.sensitivity = sensitivity;
        PlayerPrefs.SetFloat(PlayerPrefsKeys.sensitivityKey, sensitivity);
    }

    public void SetLocation(Vector3 location)
    {
        _firstPersonController.SetLocation(location);
    }

    private void TryInteractWith(IInteractable interactable)
    {
        if (interactable.MetAllConditionsToInteract())
        {
            interactable.Interact();

            if (interactable is ITimedInteractable)
            {
                OnTimedInteractableInteraction?.Invoke(interactable, EventArgs.Empty);
            }
        }
    }

    private void OnMovement(InputValue value)
    {
        if (!isMovementSuspended)
        {
            _firstPersonController.moveInput = value.Get<Vector2>();
        }
    }

    private void OnLook(InputValue value)
    {
        _firstPersonController.lookInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            _firstPersonController.TryJump();
        }
    }
}
