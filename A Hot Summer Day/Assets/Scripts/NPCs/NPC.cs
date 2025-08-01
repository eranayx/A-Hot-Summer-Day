using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class NPC : MonoBehaviour, IInteractable
{
    public enum RotationOptions
    {
        NoRotate,
        RotateBeforeMoving,
        RotateAfterMoving
    }

    public Shop DesignatedShop { get; private set; } = null;
    public CustomerQueue DesignatedQueue { get; private set; } = null;
    public CustomerDespawner DesignatedDespawner { get; private set; } = null;

    protected Animator animator;
    
    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");

    private Coroutine _movementCoroutine;
    private Coroutine _rotationCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // ! Should probably combine into one function !
    public void SetShop(Shop shop)
    {
        DesignatedShop = shop;
    }

    public void SetDespawner(CustomerDespawner despawner)
    {
        DesignatedDespawner = despawner;
    }

    public void SetQueue(CustomerQueue queue)
    {
        DesignatedQueue = queue;
    }

    public void MoveTo(Vector3 newPosition, RotationOptions rotationOption = RotationOptions.NoRotate)
    {
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
        }

        _movementCoroutine = StartCoroutine(MoveToCoroutine(newPosition, rotationOption));
    }

    public void RotateTo(Vector3 targetPosition)
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
        }

        _rotationCoroutine = StartCoroutine(RotateToCoroutine(targetPosition));
    }

    protected IEnumerator MoveToCoroutine(Vector3 newPosition, RotationOptions rotationOption)
    {
        float speed = 2f;
        float distanceCutoff = 0.1f;

        if (rotationOption == RotationOptions.RotateBeforeMoving)
        {
            RotateTo(newPosition);
        }

        animator.SetBool(isWalkingHash, true);

        while (Vector3.Distance(transform.position, newPosition) > distanceCutoff)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = newPosition;

        animator.SetBool(isWalkingHash, false);

        if (rotationOption == RotationOptions.RotateAfterMoving)
        {
            RotateTo(newPosition);
        }
    }

    // ! needs debugging (coffee shop) !
    protected IEnumerator RotateToCoroutine(Vector3 targetPosition)
    {
        float speed = 5f;
        float rotationAngleCutoff = 1;

        Vector3 targetDirection = targetPosition - transform.position;
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        while (Quaternion.Angle(transform.rotation, targetRotation) > rotationAngleCutoff)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public abstract void Interact();
    public abstract bool MetAllConditionsToInteract();
    public abstract string GetInteractText();
    public abstract Transform GetTransform();
}
