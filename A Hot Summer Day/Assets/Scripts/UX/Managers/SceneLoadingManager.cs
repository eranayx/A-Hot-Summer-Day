using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingManager : MonoBehaviour
{
    public static SceneLoadingManager Instance { get; private set; }

    private enum SceneIndices
    {
        LoadingScreen = 0,
        TitleScreen = 1,
        GameScene = 2
    }

    private readonly List<AsyncOperation> _scenesLoading = new();
 
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Image _progressBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.LoadSceneAsync((int)SceneIndices.TitleScreen, LoadSceneMode.Additive);
        }
        else
        {
            Destroy(Instance);
        }
    }

    // Credits: https://www.youtube.com/watch?v=iXWFTgFNRdM (not including fake progress bar)
    public void LoadGame()
    {
        _loadingScreen.SetActive(true);

        _scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndices.TitleScreen));
        _scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndices.GameScene, LoadSceneMode.Additive));

        StartCoroutine(LoadGameCoroutine());
    }

    private IEnumerator LoadGameCoroutine()
    {
        Time.timeScale = 0;  // To ensure customers do not spawn during loading screen

        float timer = 0;
        float quickLoadingTime = 1f;

        foreach (AsyncOperation sceneLoading in _scenesLoading)
        {
            while (!sceneLoading.isDone)
            {
                float totalLoadProgress = 0;

                for (int i = 0; i < _scenesLoading.Count; i++)
                {
                    totalLoadProgress += _scenesLoading[i].progress / _scenesLoading.Count;
                }

                timer += Time.unscaledDeltaTime;
                _progressBar.fillAmount = totalLoadProgress;
 
                yield return null;
            }
        }

        if (timer < quickLoadingTime)
        {
            _progressBar.fillAmount = 0;
            yield return StartCoroutine(FakeProgressBar());
        }

        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        _loadingScreen.SetActive(false);
    }

    // Fakes a loading screen if the game loads too fast (for visual effects)
    private IEnumerator FakeProgressBar()
    {
        float delay = 0.35f;

        _progressBar.fillAmount = Random.Range(0.1f, 0.5f);
        yield return new WaitForSecondsRealtime(delay);
        _progressBar.fillAmount = Random.Range(_progressBar.fillAmount, 0.75f);
        yield return new WaitForSecondsRealtime(delay);
        _progressBar.fillAmount = Random.Range(_progressBar.fillAmount, 0.85f);
        yield return new WaitForSecondsRealtime(delay);

        float targetVisualFillAmount = 1;
        float lerpCutoff = 0.001f;
        float speed = 20f;

        while (targetVisualFillAmount - _progressBar.fillAmount > lerpCutoff)
        {
            _progressBar.fillAmount = Mathf.Lerp(_progressBar.fillAmount, targetVisualFillAmount, speed * Time.unscaledDeltaTime);
            yield return null;
        }

        _progressBar.fillAmount = targetVisualFillAmount;
    }
}
