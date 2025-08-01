using System;
using System.Collections;
using UnityEngine;

public class LemonadeMixer : MonoBehaviour, ITimedInteractable
{
    public class OnInteractedEventArgs : EventArgs
    {
        public float SFXduration;
    }

    public static event EventHandler<OnInteractedEventArgs> OnInteracted;

    private Coroutine _makeLemonadeCoroutine;

    [SerializeField] private GameObject _lemonade;


    public static void NPCPlayInteractionSound(float duration)
    {
        OnInteracted?.Invoke(null, new OnInteractedEventArgs { SFXduration = duration });
    }

    public void Interact()
    {
        float interactionTime = GetInteractionTime();

        _makeLemonadeCoroutine ??= StartCoroutine(MakeLemonade(interactionTime));
        OnInteracted?.Invoke(this, new OnInteractedEventArgs { SFXduration = interactionTime });
    }

    public bool MetAllConditionsToInteract()
    {
        return !Player.Instance.IsHoldingItem;
    }

    public string GetInteractText()
    {
        return "Make Lemonade";
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
        return "Making lemonade...";
    }

    private IEnumerator MakeLemonade(float seconds)
    {
        Player.Instance.isMovementSuspended = true;

        yield return new WaitForSeconds(seconds);

        Player.Instance.HoldItem(_lemonade);
        Player.Instance.isMovementSuspended = false;

        _makeLemonadeCoroutine = null;
    }
}
    
