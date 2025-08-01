using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Coroutine _notificationCoroutine;
    private bool _isAdjustingSettings = false;
    private readonly Dictionary<string, bool> _validCodes = new()
    {
        { "release", false },
        { "bobthebuilder", false },
        { "sakura bloom", false }
    };

    [Header("Settings")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private GameObject _settingsCanvas;

    [Space]
    [Header("Notifications")]
    [SerializeField] private CanvasGroup _notificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI _notificationsText;

    [Space]
    [Header("Special")]
    [SerializeField] private Portal _sakuraPortal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    private void Start()
    {
        _settingsButton.onClick.AddListener(() => ShowSettings());
        LightingManager.Instance.OnNewDay += Lighting_OnNewDay;
        Shop.OnUpgraded += Shop_OnUpgraded;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {            
            if (!Player.Instance.InteractingWithUI)
            {
                ShowSettings();
            }
            else if (_isAdjustingSettings)
            {
                HideSettings();
            }
        }
    }

    public void SendNotification(string text, float duration=5, bool overridePlayerSettings=false)
    {
        if (Player.Instance.isNotificationsEnabled || overridePlayerSettings)
        {
            _notificationsText.SetText(text);
            _notificationCoroutine ??= StartCoroutine(ShowNotification(duration));
        }                   
    }

    // ! Refactor: probably a better way to do this !
    public void ValidateCode(string code)
    {
        float notificationDuration = 1;
        code = code.ToLower();        

        if (!_validCodes.ContainsKey(code))
        {
            SendNotification("Invalid Code!", duration: notificationDuration);
            return;
        }

        if (HasRedeemedCode(code))
        {
            SendNotification("Code already redeemed!", duration: notificationDuration);
            return;
        }

        float money;
        if (code == "release")
        {
            money = 50;

            Player.Instance.AddMoney(money, isNotification: true);
            SendNotification($"+${money}!", duration: notificationDuration);
            _validCodes[code] = true;
        }
        else if (code == "bobthebuilder")
        {
            money = 100;

            Player.Instance.AddMoney(money, isNotification: true);
            SendNotification($"+${money}!", duration: notificationDuration);
            _validCodes[code] = true;
        }

        // Player can return here
        else if (code == "sakura bloom")
        {
            _sakuraPortal.SetReturnLocation(Player.Instance.transform.position);
            Player.Instance.SetLocation(_sakuraPortal.transform.position);            
            SendNotification("Woah, congrats on finding this place!\n" +
                "Unfortunately, this was really last minute ;-;\n" +
                "(last year's looked better)");
        }        
    }

    private bool HasRedeemedCode(string code)
    {
        return _validCodes[code];
    }

    // Gives the player coins on every new day
    // ! Needs to be proportionate to their game progress !
    private void Lighting_OnNewDay(object sender, System.EventArgs e)
    {
        float freeCoins = Random.Range(75, 150);
        string notification = $"Bank dividends!\n\nThe bank just gave you ${freeCoins:F2} to support your journey!";

        Player.Instance.AddMoney(freeCoins, isNotification: true);
        SendNotification(notification);
    }

    private void Shop_OnUpgraded(object sender, Shop.OnUpgradeEventArgs e)
    {
        if (!e.isMaxLevel)
        {
            return;
        }

        string notification = string.Empty;

        if (e.shop is LemonadeStand)
        {
            notification = "Congratulations on starting your mini-business." +
                " Give yourself a pat on the back for helping your community." +
                " Even if you're not selling lemonade on a hot summer day," +
                " considering volunteering at your local food pantry or library." +
                " It makes the world a better place! Please feel free to continue playing!";

                
        }
        else if (e.shop is CoffeeShop)
        {
            notification = "If you've made it this far, I applaud you for your dedication" +
                " This area was (and is) still under development as of release, so prices are unbalanced." +
                " Thank you for playing!";
        }

        SendNotification(notification, 30, true);        
    }

    private void ShowSettings()
    {
        _isAdjustingSettings = true;
        _settingsCanvas.SetActive(true);
        Player.Instance.UnlockCursor();  
    }

    private void HideSettings()
    {
        _isAdjustingSettings = false;
        _settingsCanvas.SetActive(false);
        Player.Instance.LockCursor();
    }

    private IEnumerator ShowNotification(float duration)
    {
        yield return FadeNotification(1);
        yield return new WaitForSeconds(duration);
        yield return FadeNotification(0);
        _notificationCoroutine = null;
    }

    private IEnumerator FadeNotification(float targetAlpha)
    {
        while (Mathf.Abs(_notificationCanvasGroup.alpha - targetAlpha) > 0.005)
        {
            _notificationCanvasGroup.alpha = Mathf.Lerp(_notificationCanvasGroup.alpha, targetAlpha, 5 * Time.deltaTime);
            yield return null;
        }

        _notificationCanvasGroup.alpha = targetAlpha;
    }
}
