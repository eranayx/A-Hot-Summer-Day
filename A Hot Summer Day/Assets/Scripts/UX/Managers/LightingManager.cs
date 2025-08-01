using System;
using UnityEngine;

//[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    private const float NEW_DAY = 8;

    public static LightingManager Instance { get; private set; }

    public event EventHandler OnNewDay;

    private bool _newDayTriggered = false;

    [SerializeField] private Light _directionalLight;
    [SerializeField] private LightingPreset _lightingPreset;
    [SerializeField, Range(0, 24)] private float _timeOfDay;
    [SerializeField, Range(24, 600)] float _dayLengthInSeconds;
    [SerializeField, Range(0, 3)] float _intensityMultiplier;

    private void OnValidate()
    {
        if (_directionalLight != null)
        {
            return;
        }

        if (RenderSettings.sun != null)
        {
            _directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lightsArray = FindObjectsOfType<Light>();
            foreach (Light light in lightsArray)
            {
                if (light.type == LightType.Directional)
                {
                    _directionalLight = light;
                    return;
                }
            }
        }
    }

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
        _timeOfDay = NEW_DAY + 0.5f;
    }

    private void Update()
    {
        if (_lightingPreset == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            _timeOfDay = (_timeOfDay + Time.deltaTime / (_dayLengthInSeconds / 24)) % 24;
        }

        UpdateLighting(_timeOfDay * (_dayLengthInSeconds / 24) / _dayLengthInSeconds);
    }

    private void UpdateLighting(float timePercentage)
    {
        RenderSettings.ambientLight = _lightingPreset.ambientColor.Evaluate(timePercentage);
        RenderSettings.fogColor = _lightingPreset.fogColor.Evaluate(timePercentage);

        if (_directionalLight != null)
        {
            _directionalLight.color = _lightingPreset.directionalColor.Evaluate(timePercentage);            
            _directionalLight.intensity = _lightingPreset.directionalIntensity.Evaluate(timePercentage) * _intensityMultiplier;
            _directionalLight.transform.rotation = Quaternion.Euler(new Vector3(timePercentage * 360 - 90, 90, 0));
        }

        if (!_newDayTriggered && IsNearPercentage(timePercentage, NEW_DAY / 24))
        {
            OnNewDay?.Invoke(this, EventArgs.Empty);
            _newDayTriggered = true;
        }

        if (_newDayTriggered && IsNearPercentage(timePercentage, 7 / 24))
        {
            _newDayTriggered = false;
        }
    }

    // timePercentage in UpdateLighting changes too quick to ever equal a certain number,
    // so returning true within a certain range worked better
    private bool IsNearPercentage(float current, float target)
    {
        float offset = target * 0.01f;
        return current >= target - offset && current <= target + offset;
    }
}

// Credits: https://www.youtube.com/watch?v=m9hj9PdO328
