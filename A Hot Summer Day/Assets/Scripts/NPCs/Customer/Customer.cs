using UnityEngine;

public class Customer : NPC
{
    private static float _defaultTipChance = 10f;

    public static float DefaultTipChance
    { 
        get => _defaultTipChance;

        private set 
        {
            _defaultTipChance = Mathf.Clamp(_defaultTipChance, 0, 1); 
        }
    }

    public static float DefaultTipPercentageMinimum { get; private set; } = 10;
    public static float DefaultTipPercentageMaximum { get; private set; } = 15;

    public override void Interact()
    {
        if (MetAllConditionsToInteract())
        {
            Player.Instance.ReleaseItem();
            SellItem();
        }
    }

    public override bool MetAllConditionsToInteract()
    {
        return DesignatedQueue.Peek() == this
            && transform.position == DesignatedQueue.FirstQueuePosition
            && Player.Instance.IsHoldingItem;
    }

    public override string GetInteractText()
    {
        return "Sell to customer";
    }

    public override Transform GetTransform()
    {
        return transform;
    }

    public void MoveToDespawn()
    {
        MoveTo(DesignatedDespawner.transform.position, RotationOptions.RotateBeforeMoving);
    }

    public void SellItem()
    {
        SellItem(DefaultTipChance, DefaultTipPercentageMinimum, DefaultTipPercentageMaximum);
    }

    public void SellItem(float tipChance, float tipPercentageMinimum, float tipPercentageMaximum)
    {
        float tipPercentage = 0;
        bool recievedTip = Random.Range(1, 100) <= tipChance;

        if (recievedTip)
        {
            tipPercentage += Random.Range(tipPercentageMinimum, tipPercentageMaximum);
        }

        Player.Instance.AddMoney(DesignatedShop.GetCurrentSellPrice() * (1 + tipPercentage / 100), recievedTip);

        DesignatedQueue.Dequeue();
    }
}
