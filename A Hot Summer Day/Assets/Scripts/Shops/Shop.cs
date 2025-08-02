using System;
using System.Collections.Generic;
using UnityEngine;

// Many references can be a GetComponentInParent instead of [SerializeField]
// Would like to increase customer spawn count / decrease customer delay per level

public abstract class Shop : MonoBehaviour
{
    public class OnUpgradeEventArgs : EventArgs
    {
        public Shop shop;  // Used to determine what notification should be sent
        public bool isMaxLevel = false;
        public bool hiredWorker = false;
    }

    public static event EventHandler<OnUpgradeEventArgs> OnUpgraded;

    public Vector3 CashierLocation { get; private set; }
    public Vector3 WorkerInteractableLocation { get; private set; }
    public bool IsWorkerHired { get; private set; } = false;
    public readonly string shopName;

    protected enum ShopLevel
    {
        Level_0,
        Level_1,
        Level_2,
        Level_3,
        Level_4,
        MaxLevel
    }

    protected readonly struct ShopUpgradeData
    {
        public readonly int cost;
        public readonly float sellPrice;
        public readonly string description;

        public ShopUpgradeData(int cost, float sellPrice, string description)
        {
            this.cost = cost;
            this.sellPrice = sellPrice;
            this.description = description;
        }
    }

    private readonly Dictionary<ShopLevel, ShopUpgradeData> _shopUpgradeDict;
    private readonly int _workerHirePrice;
    private readonly float _workerMultiplierBonus;
    private float _shopMultipler = 1;
    private ShopLevel _currentLevel = ShopLevel.Level_0;
    private ShopLevel _nextLevel = ShopLevel.Level_1;

    protected Shop(Dictionary<ShopLevel, ShopUpgradeData> shopUpgradeDataDict, string shopName, int workerHirePrice, float workerMultiplierBonus)
    {
        _shopUpgradeDict = shopUpgradeDataDict;
        this.shopName = shopName;
        _workerHirePrice = workerHirePrice;
        _workerMultiplierBonus = workerMultiplierBonus;
    }

    protected void SetCashierLocation(Vector3 cashierLocation)
    {
        CashierLocation = cashierLocation;
    }

    protected void SetWorkerInteractableLocation(Vector3 workerInteractableLocation)
    {
        WorkerInteractableLocation = workerInteractableLocation;
    }

    public void TryUpgrade()
    {
        if (HasEnoughMoney(_shopUpgradeDict[_nextLevel].cost) && !IsMaxLevel())
        {
            Player.Instance.AddMoney(_shopUpgradeDict[_nextLevel].cost * -1);
            IncrementLevel();

            OnUpgraded?.Invoke(this, new OnUpgradeEventArgs { shop = this, isMaxLevel = IsMaxLevel() });
        }
    }

    public bool TryHireWorker()
    {
        if (HasEnoughMoney(GetWorkerHirePrice()) && !IsWorkerHired)
        {
            Player.Instance.AddMoney(GetWorkerHirePrice() * -1);
            IncreaseShopMultiplier(_workerMultiplierBonus);
            OnUpgraded?.Invoke(this, new OnUpgradeEventArgs { hiredWorker = true });

            IsWorkerHired = true;
            return true;
        }

        return false;
    }

    public void IncreaseShopMultiplier(float multiplier)
    {
        _shopMultipler += multiplier;
    }

    public float GetCurrentSellPrice()
    {
        return _shopUpgradeDict[_currentLevel].sellPrice * _shopMultipler;
    }

    public int GetWorkerHirePrice()
    {
        return _workerHirePrice;
    }

    public string GetUpgradeDescription()
    {
        if (IsMaxLevel())
        {
            return $"Congratulations on fully upgrading your {shopName}";
        }

        return _shopUpgradeDict[_nextLevel].description;
    }

    public string GetSellPriceChangeText()
    {
        string description = $"Sell Price\n${_shopUpgradeDict[_currentLevel].sellPrice * _shopMultipler:F2}";

        if (!IsMaxLevel())
        {
            description += $" > ${_shopUpgradeDict[_nextLevel].sellPrice * _shopMultipler:F2}";
        }

        return description;
    }

    public string GetWorkerBonusText()
    {
        float multiplierToShow = IsWorkerHired ? _shopMultipler : _shopMultipler + _workerMultiplierBonus;
        string text = $"Worker Bonus: {multiplierToShow:F2}x Income";

        if (!IsWorkerHired)
        {
            return "<s>" + text + "</s>";
        }

        return text;
    }

    public string GetUpgradeCostText()
    {
        if (IsMaxLevel())
        {
            return "Max Level!";
        }

        return $"Upgrade [${_shopUpgradeDict[_nextLevel].cost:F2}]";
    }

    private void IncrementLevel()
    {
        _currentLevel = _nextLevel;

        if (!IsMaxLevel())
        {
            _nextLevel++;
        }        
    }

    private bool IsMaxLevel()
    {
        return _currentLevel == ShopLevel.MaxLevel;
    }

    private bool HasEnoughMoney(float moneyNeeded)
    {
        return Player.Instance.Money > moneyNeeded;
    }
}
