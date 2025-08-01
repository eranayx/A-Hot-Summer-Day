using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUpgradeUI : MonoBehaviour
{
    private Worker _worker;

    [SerializeField] private TextMeshProUGUI _upgradeNameText;
    [SerializeField] private TextMeshProUGUI _upgradeDescriptionText;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private Image _upgradeProgressBar;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private WorkerStats.WorkerUpgradeType _upgradeType;

    private void Awake()
    {
        _worker = GetComponentInParent<Worker>(true);

        if (_worker == null)
        {
            throw new System.Exception("Unable to get [Worker] component from parent objects.");
        }
    }

    private void Start()
    {
        UpdateUpgradeUI();

        _worker.workerStats.OnUpgraded += Worker_OnUpgraded;
        _upgradeNameText.SetText(_worker.workerStats.GetUpgradeNameText(_upgradeType));
        _upgradeButton.onClick.AddListener(() => _worker.workerStats.TryUpgrade(_upgradeType));
    }

    // Ensure progress bar is updated even if UI GameObject was setActive(false)
    // and ProgressBarLerp did not finish running
    private void OnEnable()
    {
        float fillPercentage = _worker.workerStats.GetUpgradeProgressNormalized(_upgradeType);

        if (_upgradeProgressBar.fillAmount != fillPercentage)
        {
            _upgradeProgressBar.fillAmount = fillPercentage;
        }
    }

    private void Worker_OnUpgraded(object sender, System.EventArgs e)
    {
        UpdateUpgradeUI();
    }

    private void UpdateUpgradeUI()
    {
        float fillPercentage = _worker.workerStats.GetUpgradeProgressNormalized(_upgradeType);
        StartCoroutine(ProgressBarLerp(fillPercentage));

        _upgradeDescriptionText.SetText(_worker.workerStats.GetUpgradeDescription(_upgradeType));
        _upgradeCostText.SetText(_worker.workerStats.GetUpgradeCostText(_upgradeType));
    }

    // Gradually fills the progress bar UI to reflect an upgrade
    private IEnumerator ProgressBarLerp(float targetPercentage)
    {
        float lerpCutoff = 0.0025f;
        float speed = 4f;

        while (targetPercentage - _upgradeProgressBar.fillAmount > lerpCutoff)
        {
            _upgradeProgressBar.fillAmount = Mathf.Lerp(_upgradeProgressBar.fillAmount, targetPercentage, speed * Time.deltaTime);
            yield return null;
        }

        _upgradeProgressBar.fillAmount = targetPercentage;
    }
}
