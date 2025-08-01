using System.Collections.Generic;
using UnityEngine;

public class LemonadeStand : Shop
{
    private static readonly Dictionary<ShopLevel, ShopUpgradeData> _shopUpgradeDict = new()
    {
        { ShopLevel.Level_0,
            new ShopUpgradeData(cost: 0, sellPrice: 1.5f, description: "default level") },
        { ShopLevel.Level_1,
            new ShopUpgradeData(cost: 5, sellPrice: 2f, description: "Freshen up your plain ice water with lemons!") },
        { ShopLevel.Level_2,
            new ShopUpgradeData(cost: 10, sellPrice : 2.5f, description: "Sweeten up the drink with simple sugar.") },
        { ShopLevel.Level_3,
            new ShopUpgradeData(cost : 25, sellPrice : 3f, description: "Use organic sugar since everything is better organic of course.") },
        { ShopLevel.Level_4,
            new ShopUpgradeData(cost : 45, sellPrice : 3.5f, description: "Buy pots and pans to improve quality with a homemade syrup.") },
        { ShopLevel.MaxLevel,
            new ShopUpgradeData(cost : 75, sellPrice : 5f, description: "Take some knife lessons to garnish with a fresh slice of lemon!") },
    };

    private static readonly string _shopName = "lemonade stand";
    private static readonly int _workerHirePrice = 35;
    private static readonly float _workerMultiplierBonus = 0.5f;

    [SerializeField] private Transform _cashierLocation;
    [SerializeField] private Transform _workerInteractableLocation;

    private LemonadeStand() : base(_shopUpgradeDict, _shopName, _workerHirePrice, _workerMultiplierBonus) { }

    private void OnValidate()
    {
        SetCashierLocation(_cashierLocation.position);
        SetWorkerInteractableLocation(_workerInteractableLocation.position);
    }
}
