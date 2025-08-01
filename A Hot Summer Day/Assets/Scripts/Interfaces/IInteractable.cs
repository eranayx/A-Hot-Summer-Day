using UnityEngine;

public interface IInteractable
{
    void Interact();
    bool MetAllConditionsToInteract();
    string GetInteractText();
    Transform GetTransform();

}
