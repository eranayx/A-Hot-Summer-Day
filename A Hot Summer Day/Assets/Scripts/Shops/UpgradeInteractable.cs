using UnityEngine;

public class UpgradeInteractable : MonoBehaviour, IInteractable
{
    private Shop _shop;
    [SerializeField] private GameObject _upgradeUI;

    private void Awake()
    {
        _shop = GetComponentInParent<Shop>();
    }

    public void Interact()
    {
        _upgradeUI.SetActive(true);
        Player.Instance.UnlockCursor();
    }

    public bool MetAllConditionsToInteract()
    {
        return !Player.Instance.InteractingWithUI;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public string GetInteractText()
    {
        return $"Upgrade {_shop.shopName}";
    }    
}
