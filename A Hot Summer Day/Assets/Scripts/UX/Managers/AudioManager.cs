using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;    

public class AudioManager : MonoBehaviour
{
    private const float VOLUME_CHANGE_SENSITIVITY = 30;

    private static readonly string MUSIC_PARAMETER = "music";
    private static readonly string SFX_PARAMETER = "sfx";

    public static AudioManager Instance { get; private set; }
    private static Coroutine _makingLemonadeCoroutine = null;
    private static Coroutine _makingCoffeeCoroutine = null;
    
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Background Music")]
    [SerializeField] private AudioSource _lemonadeShopBGM;
    [SerializeField] private AudioSource _coffeeShopBGM;
    [SerializeField] private AudioSource _sakuraGardenBGM;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource _makingLemonadeSFX;
    [SerializeField] private AudioSource _shopUpgradeSFX;
    [SerializeField] private AudioSource _moneyChangedSFX;

    [Space]
    [SerializeField] private AudioClip _soldItemSFX;
    [SerializeField] private AudioClip _tipSFX;
    [SerializeField] private List<AudioSource> _cashierLocations;

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
        Shop.OnUpgraded += Shop_OnUpgraded;
        LemonadeMixer.OnInteracted += LemonadeMixer_OnInteracted;
        EspressoMachine.OnInteracted += EspressoMachine_OnInteracted;
        Player.Instance.OnMoneyChanged += Player_OnMoneyChanged;
    }
    
    public void SetMusicVolume(float value)
    {
        _audioMixer.SetFloat(MUSIC_PARAMETER, Mathf.Log10(value) * VOLUME_CHANGE_SENSITIVITY);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.musicKey, value);
    }

    public void SetSFXVolume(float value)
    {
        _audioMixer.SetFloat(SFX_PARAMETER, Mathf.Log10(value) * VOLUME_CHANGE_SENSITIVITY);
        PlayerPrefs.SetFloat(PlayerPrefsKeys.sfxKey, value);
    }

    private void Shop_OnUpgraded(object sender, Shop.OnUpgradeEventArgs e)
    {
        _shopUpgradeSFX.Play();
    }

    // Need a louder tip SFX
    // Need to refactor to consider notification money changes
    private void Player_OnMoneyChanged(object sender, Player.OnMoneyChangedEventArgs e)
    {
        if (e.isNotification)
        {
            _moneyChangedSFX.Play();
            return;
        }

        AudioSource closestAudioSource = GetClosestCashierAudioSource();
        
        if (closestAudioSource != null)
        {
            closestAudioSource.clip = e.recievedTip ? _tipSFX : _soldItemSFX;
            closestAudioSource.Play();
        }
    }

    private void LemonadeMixer_OnInteracted(object sender, LemonadeMixer.OnInteractedEventArgs e)
    {
        _makingLemonadeCoroutine ??= StartCoroutine(MakingLemonadeRoutine(e.SFXduration));
    }

    private IEnumerator MakingLemonadeRoutine(float duration)
    {
        yield return FadeSFX(_makingLemonadeSFX, duration);
        _makingLemonadeCoroutine = null;
    }

    private void EspressoMachine_OnInteracted(object sender, EspressoMachine.OnInteractedEventArgs e)
    {
        _makingCoffeeCoroutine ??= StartCoroutine(MakingCoffeeCoroutine(e.SFXduration));
    }

    // Temporarily reuse same SFX
    private IEnumerator MakingCoffeeCoroutine(float duration)
    {
        yield return FadeSFX(_makingLemonadeSFX, duration);
        _makingCoffeeCoroutine = null;

    }

    // Can be split into two coroutine calls
    private IEnumerator FadeSFX(AudioSource source, float totalDuration)
    {
        float originalVolume = source.volume;
        float targetVolume = originalVolume;
        float lerpCutoff = 0.001f;
        float elapsedTime = 0;
        float fadeDuration = 0.05f;

        source.Play();
        source.volume = 0;

        while (targetVolume - source.volume > lerpCutoff)
        {
            source.volume = Mathf.Lerp(source.volume, targetVolume, elapsedTime / fadeDuration * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        source.volume = targetVolume;
        yield return new WaitForSeconds(totalDuration - fadeDuration - elapsedTime);

        targetVolume = 0;
        elapsedTime = 0;
        while (source.volume - targetVolume > lerpCutoff)
        {
            source.volume = Mathf.Lerp(source.volume, targetVolume, elapsedTime / fadeDuration * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        source.Stop();
        source.volume = originalVolume;
    }

    private AudioSource GetClosestCashierAudioSource()
    {
        AudioSource closestAudioSource = null;

        foreach (AudioSource source in _cashierLocations)
        {
            if (closestAudioSource == null)
            {
                closestAudioSource = source;
            }

            if (Vector3.Distance(transform.position, source.transform.position)
                < Vector3.Distance(transform.position, closestAudioSource.transform.position))
            {
                closestAudioSource = source;
            }
        }

        return closestAudioSource;
    }
}
