using System;
using System.Collections.Generic;

public class WorkerStats
{
    public event EventHandler OnUpgraded;

    public enum WorkerUpgradeType
    {
        WorkerSpeed,
        TipChance,
        TipPercentage
    }

    public enum WorkerUpgradeLevel
    {
        Level_0,
        Level_1,
        Level_2,
        Level_3,
        Level_4,
        Level_5,
        MaxLevel
    }

    public readonly struct WorkerUpgradeData
    {
        public readonly int cost;
        public readonly float data;

        public WorkerUpgradeData(int cost, float data)
        {
            this.cost = cost;
            this.data = data;
        }
    }

    private readonly Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> _speedUpgradeDict;
    private WorkerUpgradeLevel _currentSpeedLevel = WorkerUpgradeLevel.Level_0;
    private WorkerUpgradeLevel _nextSpeedLevel = WorkerUpgradeLevel.Level_1;

    private readonly Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> _tipChanceUpgradeDict;
    private WorkerUpgradeLevel _currentTipChanceLevel = WorkerUpgradeLevel.Level_0;
    private WorkerUpgradeLevel _nextTipChanceLevel = WorkerUpgradeLevel.Level_1;

    private readonly Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> _tipPercentageUpgradeDict;
    private WorkerUpgradeLevel _currentTipPercentageLevel = WorkerUpgradeLevel.Level_0;
    private WorkerUpgradeLevel _nextTipPercentageLevel = WorkerUpgradeLevel.Level_1;

    private float _workerSpeed = 3f;

    public WorkerStats(
        Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> speedUpgradeDict,
        Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> tipChanceUpgradeDict,
        Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> tipPercentageUpgradeDict)
    {
        _speedUpgradeDict = speedUpgradeDict;
        _tipChanceUpgradeDict = tipChanceUpgradeDict;
        _tipPercentageUpgradeDict = tipPercentageUpgradeDict;
    }

    // Upgrades the worker's stat if the player has sufficient coins
    public void TryUpgrade(WorkerUpgradeType upgradeType)
    {
        if (IsMaxLevel(upgradeType))
        {
            return;
        }

        if (!HasEnoughMoneyToUpgrade(upgradeType))
        {
            return;
        }

        Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> upgradeTypeDict = GetDictionaryRef(upgradeType);
        WorkerUpgradeLevel nextLevelRef = GetNextLevelRef(upgradeType);

        Player.Instance.AddMoney(upgradeTypeDict[nextLevelRef].cost * -1);
        if (upgradeType == WorkerUpgradeType.WorkerSpeed)
        {
            _workerSpeed = _speedUpgradeDict[_nextSpeedLevel].data;
        }

        IncrementLevel(upgradeType);
        OnUpgraded?.Invoke(this, EventArgs.Empty);
    }

    public string GetUpgradeNameText(WorkerUpgradeType upgradeType)
    {
        return upgradeType switch
        {
            WorkerUpgradeType.WorkerSpeed => "Work Speed",
            WorkerUpgradeType.TipChance => "Tip Chance",
            WorkerUpgradeType.TipPercentage => "Tip Percentage",
            _ => throw new Exception("invalid worker upgrade type")
        };
    }

    // Returns the appropriate description for each upgrade type
    public string GetUpgradeDescription(WorkerUpgradeType upgradeType)
    {
        Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> upgradeTypeDict = GetDictionaryRef(upgradeType);
        WorkerUpgradeLevel currentLevelRef = GetCurrentLevelRef(upgradeType);
        WorkerUpgradeLevel nextLevelRef = GetNextLevelRef(upgradeType);

        float currentUpgradeData = upgradeTypeDict[currentLevelRef].data;
        float nextUpgradeData = upgradeTypeDict[nextLevelRef].data;
        string description = string.Empty;

        if (upgradeType == WorkerUpgradeType.WorkerSpeed)
        {
            description += $"{currentUpgradeData}s";
        }
        else if (upgradeType == WorkerUpgradeType.TipChance)
        {
            description += $"{Customer.DefaultTipChance + currentUpgradeData}%";
        }
        else if (upgradeType == WorkerUpgradeType.TipPercentage)
        {
            description += $"{Customer.DefaultTipPercentageMinimum + currentUpgradeData}%-{Customer.DefaultTipPercentageMaximum + currentUpgradeData}%";
        }
        else
        {
            throw new Exception("invalid worker upgrade type");
        }

        if (!IsMaxLevel(upgradeType))
        {
            return upgradeType switch
            {
                WorkerUpgradeType.WorkerSpeed => description += $" > {nextUpgradeData}s",
                WorkerUpgradeType.TipChance => description += $" > {Customer.DefaultTipChance + nextUpgradeData}%",
                WorkerUpgradeType.TipPercentage =>
                    description += $" > {Customer.DefaultTipPercentageMinimum + nextUpgradeData}%-{Customer.DefaultTipPercentageMaximum + nextUpgradeData}%",
                _ => throw new Exception("invalid worker upgrade type")
            };
        }

        return description;   
    }

    // Returns the ratio between current level and total levels in the stat
    public float GetUpgradeProgressNormalized(WorkerUpgradeType upgradeType)
    {
        if (IsMaxLevel(upgradeType))
        {
            return 1;
        }

        Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> upgradeTypeDict = GetDictionaryRef(upgradeType);
        WorkerUpgradeLevel currentLevelRef = GetCurrentLevelRef(upgradeType);

        return (float) currentLevelRef / (upgradeTypeDict.Count - 1);  // Exclude the first level (level 0)
    }

    public string GetUpgradeCostText(WorkerUpgradeType upgradeType)
    {
        if (IsMaxLevel(upgradeType))
        {
            return "Max Level!";
        }

        Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> upgradeTypeDict = GetDictionaryRef(upgradeType);
        WorkerUpgradeLevel nextLevelRef = GetNextLevelRef(upgradeType);

        return $"Upgrade [${upgradeTypeDict[nextLevelRef].cost}]";
    }

    public float GetWorkerSpeed(bool getBaseSpeed=false)
    {
        if (getBaseSpeed)
        {
            return _speedUpgradeDict[WorkerUpgradeLevel.Level_0].data;
        }

        return _workerSpeed;
    }

    public float GetTipChance()
    {
        return _tipChanceUpgradeDict[_currentTipChanceLevel].data;
    }

    public float GetTipPercentageIncrease()
    {
        return _tipPercentageUpgradeDict[_currentTipPercentageLevel].data;
    }

    // Private helper functions

    private bool IsMaxLevel(WorkerUpgradeType upgradeType)
    {
        return GetCurrentLevelRef(upgradeType) == WorkerUpgradeLevel.MaxLevel;
    }

    private bool HasEnoughMoneyToUpgrade(WorkerUpgradeType upgradeType)
    {
        Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> upgradeTypeDict = GetDictionaryRef(upgradeType);
        WorkerUpgradeLevel nextLevelRef = GetNextLevelRef(upgradeType);

        return Player.Instance.Money >= upgradeTypeDict[nextLevelRef].cost;
    }

    private void IncrementLevel(WorkerUpgradeType upgradeType)
    {
        if (IsMaxLevel(upgradeType))
        {
            return;
        }

        Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> upgradeTypeDict = GetDictionaryRef(upgradeType);
        ref WorkerUpgradeLevel currentLevelRef = ref GetCurrentLevelRef(upgradeType);
        ref WorkerUpgradeLevel nextLevelRef = ref GetNextLevelRef(upgradeType);

        currentLevelRef = nextLevelRef;

        if (IsMaxLevel(upgradeType))
        {
            return;
        }

        while (!upgradeTypeDict.ContainsKey(nextLevelRef + 1))
        {
            nextLevelRef++;
        }

        nextLevelRef++;
    }

    private Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> GetDictionaryRef(WorkerUpgradeType upgradeType)
    {
        return upgradeType switch
        {
            WorkerUpgradeType.WorkerSpeed => _speedUpgradeDict,
            WorkerUpgradeType.TipChance => _tipChanceUpgradeDict,
            WorkerUpgradeType.TipPercentage => _tipPercentageUpgradeDict,
            _ => throw new Exception("invalid worker upgrade type")
        };
    }

    private ref WorkerUpgradeLevel GetCurrentLevelRef(WorkerUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case WorkerUpgradeType.WorkerSpeed:
                return ref _currentSpeedLevel;

            case WorkerUpgradeType.TipChance:
                return ref _currentTipChanceLevel;

            case WorkerUpgradeType.TipPercentage:
                return ref _currentTipPercentageLevel;

            default:
                throw new Exception("invalid worker upgrade type");
        }
    }

    private ref WorkerUpgradeLevel GetNextLevelRef(WorkerUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case WorkerUpgradeType.WorkerSpeed:
                return ref _nextSpeedLevel;

            case WorkerUpgradeType.TipChance:
                return ref _nextTipChanceLevel;

            case WorkerUpgradeType.TipPercentage:
                return ref _nextTipPercentageLevel;

            default:
                throw new Exception("invalid worker upgrade type");
        }
    }
}
