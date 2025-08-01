using System;
using System.Collections;
using UnityEngine;

public class EspressoMachine : MonoBehaviour, ITimedInteractable
{
    public class OnInteractedEventArgs : EventArgs
    {
        public float SFXduration;
    }

    public static event EventHandler<OnInteractedEventArgs> OnInteracted;

    private Coroutine _makeCoffeeCoroutine;

    [SerializeField] private GameObject _coffee;

    public static void NPCPlayInteractionSound(float duration)
    {
        OnInteracted?.Invoke(null, new OnInteractedEventArgs { SFXduration = duration });
    }

    public void Interact()
    {
        float interactionTime = GetInteractionTime();

        _makeCoffeeCoroutine ??= StartCoroutine(MakeCoffee(interactionTime));
        OnInteracted?.Invoke(this, new OnInteractedEventArgs { SFXduration = interactionTime });
    }

    public bool MetAllConditionsToInteract()
    {
        return !Player.Instance.IsHoldingItem;
    }

    public string GetInteractText()
    {
        return "Make coffee";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public float GetInteractionTime()
    {
        return 1;
    }

    public string GetProgessBarText()
    {
        return "Making coffee...";
    }

    private IEnumerator MakeCoffee(float seconds)
    {
        Player.Instance.isMovementSuspended = true;

        yield return new WaitForSeconds(seconds);

        Player.Instance.HoldItem(_coffee);
        Player.Instance.isMovementSuspended = false;

        _makeCoffeeCoroutine = null;
    }
}
