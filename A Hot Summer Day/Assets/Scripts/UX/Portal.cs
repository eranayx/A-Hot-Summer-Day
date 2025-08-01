using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    private Vector3 _returnLocation;

    public string GetInteractText()
    {
        return "Go back";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact()
    {
        Player.Instance.SetLocation(_returnLocation);
    }

    public bool MetAllConditionsToInteract()
    {
        return true;
    }

    public void SetReturnLocation(Vector3 returnLocation)
    {
        _returnLocation = returnLocation;
    }
}
