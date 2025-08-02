using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public static bool isSettingsLoaded = false;

    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private Toggle _notificationToggle;
    [SerializeField] private TMP_InputField _codeInput;

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

    private void OnEnable()
    {
        _codeInput.text = string.Empty;
    }

    private IEnumerator Start()
    {
        _musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        _sensitivitySlider.onValueChanged.AddListener(Player.Instance.SetSensitivity);
        _notificationToggle.onValueChanged.AddListener(NotificationToggle_OnValueChanged);

        _codeInput.onSelect.AddListener((text) => 
        {
            Player.Instance.isMovementSuspended = true;
            Player.Instance.isInputtingText = true;
        });
        _codeInput.onEndEdit.AddListener((text) =>
        {
            Player.Instance.isMovementSuspended = false;
            Player.Instance.isInputtingText = false;
        });
        _codeInput.onDeselect.AddListener((text) =>
        {
            Player.Instance.isMovementSuspended = false;
            Player.Instance.isInputtingText = false;
        });
        _codeInput.onSubmit.AddListener((text) => 
        { 
            GameManager.Instance.ValidateCode(text);
            Player.Instance.isMovementSuspended = false;
            Player.Instance.isInputtingText = false;
        });

        yield return new WaitUntil(() => isSettingsLoaded);

        GetComponent<CanvasGroup>().alpha = 1.0f;
        gameObject.SetActive(false);
    }

    public void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.musicKey))
        {
            _musicSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.musicKey);
            AudioManager.Instance.SetMusicVolume(_musicSlider.value);
        }

        if (PlayerPrefs.HasKey(PlayerPrefsKeys.sfxKey))
        {
            _sfxSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.sfxKey);
            AudioManager.Instance.SetSFXVolume(_sfxSlider.value);
        }

        if (PlayerPrefs.HasKey(PlayerPrefsKeys.sensitivityKey))
        {
            _sensitivitySlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.sensitivityKey);
            Player.Instance.SetSensitivity(_sensitivitySlider.value);
        }

        if (PlayerPrefs.HasKey(PlayerPrefsKeys.notificationKey))
        {
            bool value = PlayerPrefs.GetInt(PlayerPrefsKeys.notificationKey) == 1;
            _notificationToggle.isOn = value;
            Player.Instance.isNotificationsEnabled = value;
        }

        isSettingsLoaded = true;
    }

    private void NotificationToggle_OnValueChanged(bool value)
    {
        int enabled = value == true ? 1 : 0;

        Player.Instance.isNotificationsEnabled = value;
        PlayerPrefs.SetInt(PlayerPrefsKeys.notificationKey, enabled);
    }
}
