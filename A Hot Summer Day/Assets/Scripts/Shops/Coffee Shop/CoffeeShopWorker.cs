using System.Collections.Generic;
using static WorkerStats;

public class CoffeeShopWorker : Worker
{
    public static readonly Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> workerSpeedUpgradeDict = new()
    {
        { WorkerUpgradeLevel.Level_0, new WorkerUpgradeData(cost: 0, data: 3.5f) },
        { WorkerUpgradeLevel.Level_1, new WorkerUpgradeData(cost: 50, data: 2.25f) },
        { WorkerUpgradeLevel.Level_2, new WorkerUpgradeData(cost: 100, data : 1.75f) },
        { WorkerUpgradeLevel.Level_3, new WorkerUpgradeData(cost : 125, data : 1.25f) },
        { WorkerUpgradeLevel.Level_4, new WorkerUpgradeData(cost : 145, data : 1f) },
        { WorkerUpgradeLevel.MaxLevel, new WorkerUpgradeData(cost : 170, data : 0.5f) },
    };

    public static readonly Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> tipChanceUpgradeDict = new()
    {
        { WorkerUpgradeLevel.Level_0, new WorkerUpgradeData(cost: 0, data: 3f) },
        { WorkerUpgradeLevel.Level_1, new WorkerUpgradeData(cost: 10, data: 6f) },
        { WorkerUpgradeLevel.Level_2, new WorkerUpgradeData(cost: 25, data : 9f) },
        { WorkerUpgradeLevel.Level_3, new WorkerUpgradeData(cost : 40, data : 12f) },
        { WorkerUpgradeLevel.MaxLevel, new WorkerUpgradeData(cost : 75, data : 15f) },
    };

    public static readonly Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> tipPercentageUpgradeDict = new()
    {
        { WorkerUpgradeLevel.Level_0, new WorkerUpgradeData(cost: 0, data: 2f) },
        { WorkerUpgradeLevel.Level_1, new WorkerUpgradeData(cost: 10, data: 5f) },
        { WorkerUpgradeLevel.Level_2, new WorkerUpgradeData(cost: 20, data : 8f) },
        { WorkerUpgradeLevel.Level_3, new WorkerUpgradeData(cost : 35, data : 11f) },
        { WorkerUpgradeLevel.Level_4, new WorkerUpgradeData(cost : 35, data : 14f) },
        { WorkerUpgradeLevel.Level_5, new WorkerUpgradeData(cost : 35, data : 17f) },
        { WorkerUpgradeLevel.MaxLevel, new WorkerUpgradeData(cost : 50, data : 20f) },
    };
}