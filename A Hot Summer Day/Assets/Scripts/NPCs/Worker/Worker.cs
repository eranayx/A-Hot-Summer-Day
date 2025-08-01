using System.Collections;
using UnityEngine;

public class Worker : NPC
{
    // ! Write a script to attach upgrade screen instead of adding and assigning in prefab !
    public WorkerStats workerStats;

    private Coroutine _workCoroutine;
    private bool _walkingToShopFromHire = true;

    [SerializeField] private GameObject _workerUpgradeScreen;

    private void Start()
    {
        SetWorkerStats();

        _workCoroutine = StartCoroutine(MoveToCoroutine(DesignatedShop.CashierLocation, RotationOptions.RotateBeforeMoving));
    }

    private void Update()
    {
        // When worker is first hired, ensure it has walked to cashier location at least once
        if (_walkingToShopFromHire && transform.position == DesignatedShop.CashierLocation)
        {
            _workCoroutine = null;
            _walkingToShopFromHire = false;
            StopAllCoroutines();
        }

        if (DesignatedQueue.IsCustomerPresent && _workCoroutine == null)
        {
            _workCoroutine = StartCoroutine(Work(workerStats.GetWorkerSpeed()));
        }
    }    

    public override void Interact()
    {
        _workerUpgradeScreen.SetActive(true);
        Player.Instance.UnlockCursor();
    }

    public override bool MetAllConditionsToInteract()
    {
        return !Player.Instance.InteractingWithUI;
    }

    public override string GetInteractText()
    {
        return "Upgrade worker";
    }

    public override Transform GetTransform()
    {
        return transform;
    }

    private void SetWorkerStats()
    {
        if (DesignatedShop is LemonadeStand)
        {
            workerStats = new(LemonadeStandWorker.workerSpeedUpgradeDict, LemonadeStandWorker.tipChanceUpgradeDict, LemonadeStandWorker.tipPercentageUpgradeDict);
        }
        else if (DesignatedShop is CoffeeShop)
        {
            workerStats = new(CoffeeShopWorker.workerSpeedUpgradeDict, CoffeeShopWorker.tipChanceUpgradeDict, CoffeeShopWorker.tipPercentageUpgradeDict);
        }
    }

    // Starts the work "animation" and sells item to customer if present
    private IEnumerator Work(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(MoveToCoroutine(DesignatedShop.WorkerInteractableLocation, RotationOptions.RotateBeforeMoving));

        StartCoroutine(PlayButtonPressAnimation());
        PlayInteractableSound(delay);
        yield return new WaitForSeconds(delay);

        yield return StartCoroutine(MoveToCoroutine(DesignatedShop.CashierLocation, RotationOptions.RotateBeforeMoving));
        yield return StartCoroutine(RotateToCoroutine(DesignatedQueue.Peek().transform.position));

        if (DesignatedQueue.IsCustomerPresent)
        {
            float calculatedTipChance = Customer.DefaultTipChance + workerStats.GetTipChance();
            float calculatedTipMinimum = Customer.DefaultTipPercentageMinimum + workerStats.GetTipPercentageIncrease();
            float calculatedTipMaximum = Customer.DefaultTipPercentageMaximum + workerStats.GetTipPercentageIncrease();

            DesignatedQueue.Peek().SellItem(calculatedTipChance, calculatedTipMinimum, calculatedTipMaximum);
        }

        _workCoroutine = null;
    }

    private IEnumerator PlayButtonPressAnimation()
    {
        int isWorkingHash = Animator.StringToHash("IsWorking");
        float animatorSpeedRatio = workerStats.GetWorkerSpeed(true) / workerStats.GetWorkerSpeed();  // ratio is dependent on worker speed [base / current] ? other way around ?

        animator.SetBool(isWorkingHash, true);
        animator.speed = animatorSpeedRatio;

        yield return new WaitForSeconds(workerStats.GetWorkerSpeed());

        animator.SetBool(isWorkingHash, false);
        animator.speed = 1;
    }

    private void PlayInteractableSound(float delay)
    {
        if (DesignatedShop is LemonadeStand)
        {
            LemonadeMixer.NPCPlayInteractionSound(delay);
        }
        else if (DesignatedShop is CoffeeShop)
        {
            EspressoMachine.NPCPlayInteractionSound(delay);
        }
    }
}
