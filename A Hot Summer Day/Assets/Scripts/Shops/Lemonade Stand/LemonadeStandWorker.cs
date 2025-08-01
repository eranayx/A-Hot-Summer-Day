using System.Collections.Generic;
using static WorkerStats;

public class LemonadeStandWorker : Worker
{
    public static readonly Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> workerSpeedUpgradeDict = new()
    {
        { WorkerUpgradeLevel.Level_0, new WorkerUpgradeData(cost: 0, data: 3f) },
        { WorkerUpgradeLevel.Level_1, new WorkerUpgradeData(cost: 10, data: 2f) },
        { WorkerUpgradeLevel.Level_2, new WorkerUpgradeData(cost: 20, data : 1.25f) },
        { WorkerUpgradeLevel.Level_3, new WorkerUpgradeData(cost : 30, data : 0.85f) },
        { WorkerUpgradeLevel.MaxLevel, new WorkerUpgradeData(cost : 45, data : 0.5f) },
    };

    public static readonly Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> tipChanceUpgradeDict = new()
    {
        { WorkerUpgradeLevel.Level_0, new WorkerUpgradeData(cost: 0, data: 0f) },
        { WorkerUpgradeLevel.Level_1, new WorkerUpgradeData(cost: 10, data: 2f) },
        { WorkerUpgradeLevel.Level_2, new WorkerUpgradeData(cost: 20, data : 4f) },
        { WorkerUpgradeLevel.Level_3, new WorkerUpgradeData(cost : 35, data : 6f) },
        { WorkerUpgradeLevel.Level_4, new WorkerUpgradeData(cost : 55, data : 8f) },
        { WorkerUpgradeLevel.MaxLevel, new WorkerUpgradeData(cost : 70, data : 10f) },
    };

    public static readonly Dictionary<WorkerUpgradeLevel, WorkerUpgradeData> tipPercentageUpgradeDict = new()
    {
        { WorkerUpgradeLevel.Level_0, new WorkerUpgradeData(cost: 0, data: 0f) },
        { WorkerUpgradeLevel.Level_1, new WorkerUpgradeData(cost: 10, data: 2.5f) },
        { WorkerUpgradeLevel.Level_2, new WorkerUpgradeData(cost: 25, data : 5f) },
        { WorkerUpgradeLevel.Level_3, new WorkerUpgradeData(cost : 35, data : 7.5f) },
        { WorkerUpgradeLevel.MaxLevel, new WorkerUpgradeData(cost : 45, data : 10f) },
    };
}
