using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaBorder : MonoBehaviour, IInteractable
{
    private enum UnlockArea
    {
        CoffeeShop,
        Undecided
    }

    private readonly Dictionary<UnlockArea, float> _borderCost = new()
    {
        {UnlockArea.CoffeeShop, 300 },
        {UnlockArea.Undecided, 1000 }
    };

    [SerializeField] private UnlockArea _unlockArea;
    [SerializeField] private TextMeshProUGUI _unlockText;

    private void Start()
    {
        _unlockText.SetText($"Unlock new area?\n${_borderCost[_unlockArea]}");
    }

    public string GetInteractText()
    {
        return $"Purchase next area for ${_borderCost[_unlockArea]}";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact()
    {
        if (Player.Instance.Money >= _borderCost[_unlockArea])
        {
            gameObject.SetActive(false);
            Player.Instance.AddMoney(_borderCost[_unlockArea] * -1, isNotification: true);
        }
    }

    public bool MetAllConditionsToInteract()
    {
        return true;
    }
}
