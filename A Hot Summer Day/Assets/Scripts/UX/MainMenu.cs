using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// Would like to have added a settings button in the main menu
public class MainMenu : MonoBehaviour
{
    private VisualElement ui;
    private Button _playButton;
    private Button _quitButton;

    [SerializeField] private CanvasGroup _textContainerCanvasGroup;
    [SerializeField] private AudioSource _menuBGM;
    [SerializeField] private AudioSource _bubbleSFX;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    void Start()
    {
        _playButton = ui.Q<Button>("PlayButton");
        _quitButton = ui.Q<Button>("QuitButton");

        _playButton.clicked += OnPlayButtonClicked;
        _quitButton.clicked += OnQuitButtonClicked;
    }

    private void OnPlayButtonClicked()
    {
        StartCoroutine(LoadGameWhenReady());
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }

    private IEnumerator LoadGameWhenReady()
    {
        GetComponent<UIDocument>().enabled = false;
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        StartCoroutine(FadeBGM(0.025f));
        _bubbleSFX.Play();

        yield return FadeUI();
        yield return new WaitUntil(() => _bubbleSFX.time >= _bubbleSFX.clip.length / 3);

        SceneLoadingManager.Instance.LoadGame();
    }

    private IEnumerator FadeUI()
    {
        float targetAlpha = 0;

        while (_textContainerCanvasGroup.alpha > 0.005)
        {
            _textContainerCanvasGroup.alpha = Mathf.Lerp(_textContainerCanvasGroup.alpha, targetAlpha, 5 * Time.deltaTime);
            yield return null;
        }

        _textContainerCanvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeBGM(float targetVolume)
    {
        while (_menuBGM.volume > targetVolume)
        {
            _menuBGM.volume = Mathf.Lerp(_menuBGM.volume, targetVolume, 2 * Time.deltaTime);
            yield return null;
        }

        _menuBGM.volume = targetVolume;
    }
}
