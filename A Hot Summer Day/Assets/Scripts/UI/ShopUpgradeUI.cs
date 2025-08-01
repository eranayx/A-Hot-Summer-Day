using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgradeUI : MonoBehaviour
{
    private Shop _shop;

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _sellPriceChangeText;
    [SerializeField] private TextMeshProUGUI _workerBonusText;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private Button _upgradeButton;

    private void Awake()
    {
        _shop = GetComponentInParent<Shop>(true);

        if (_shop == null)
        {
            throw new System.Exception("Unable to get [Shop] component from parent objects.");
        }
    }

    private void Start()
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        _title.SetText($"{textInfo.ToTitleCase(_shop.shopName)} Upgrades");
    }

    private void OnEnable()
    {
        UpdateUpgradeText();

        Shop.OnUpgraded += Shop_OnUpgraded;
        _upgradeButton.onClick.AddListener(() => _shop.TryUpgrade());
    }

    private void OnDisable()
    {
        Shop.OnUpgraded -= Shop_OnUpgraded;
        _upgradeButton.onClick.RemoveAllListeners();
    }

    private void Shop_OnUpgraded(object sender, Shop.OnUpgradeEventArgs e)
    {
        UpdateUpgradeText();
    }

    private void UpdateUpgradeText()
    {
        _descriptionText.SetText(_shop.GetUpgradeDescription());
        _sellPriceChangeText.SetText(_shop.GetSellPriceChangeText());
        _upgradeCostText.SetText(_shop.GetUpgradeCostText());

        _workerBonusText.SetText(_shop.GetWorkerBonusText());

        if (_shop.IsWorkerHired)
        {
            Color32 lightGreen = new(138, 188, 64, 255);
            _workerBonusText.color = lightGreen;
        }
    }
}
