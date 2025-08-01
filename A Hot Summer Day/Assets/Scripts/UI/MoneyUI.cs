using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyText;

    void Start()
    {
        UpdateMoneyText();
        Player.Instance.OnMoneyChanged += Player_OnMoneyChanged;
    }

    private void Player_OnMoneyChanged(object sender, Player.OnMoneyChangedEventArgs e)
    {
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        _moneyText.SetText($"${Player.Instance.Money:F2}");
    }
}
