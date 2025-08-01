using System.Collections.Generic;
using UnityEngine;

public class CoffeeShop : Shop
{
    private static readonly Dictionary<ShopLevel, ShopUpgradeData> _shopUpgradeDict = new()
    {
        { ShopLevel.Level_0,
            new ShopUpgradeData(cost: 0, sellPrice: 6.5f, description: "default level") },
        { ShopLevel.Level_1,
            new ShopUpgradeData(cost: 150, sellPrice: 7.5f, description: "Provide delicious croissants straight from the oven.") },
        { ShopLevel.Level_2,
            new ShopUpgradeData(cost: 200, sellPrice : 9f, description: "Offer freshly squeezed juice to customers.") },
        { ShopLevel.Level_3,
            new ShopUpgradeData(cost : 275, sellPrice : 10f, description: "Make some sandwiches and put them on display.") },
        { ShopLevel.Level_4,
            new ShopUpgradeData(cost : 400, sellPrice : 10.5f, description: "Sweeten the experience with donuts from a local bakery!") },
        { ShopLevel.MaxLevel,
            new ShopUpgradeData(cost : 450, sellPrice : 11f, description: "Place an additional order for cakes and cupcakes.") },
    };

    private static readonly string _shopName = "coffee shop";
    private static readonly int _workerHirePrice = 300;
    private static readonly float _workerMultiplierBonus = 1f;

    [SerializeField] private Transform _cashierLocation;
    [SerializeField] private Transform _workerInteractableLocation;

    private CoffeeShop() : base(_shopUpgradeDict, _shopName, _workerHirePrice, _workerMultiplierBonus) { }

    private void OnValidate()
    {
        SetCashierLocation(_cashierLocation.position);
        SetWorkerInteractableLocation(_workerInteractableLocation.position);
    }
}
