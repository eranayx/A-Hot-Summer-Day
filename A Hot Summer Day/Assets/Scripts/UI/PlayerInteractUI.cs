using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractUI : MonoBehaviour
{
    private IInteractable _closestInteractable;
    private Coroutine _progessBarCoroutine;
    private CanvasGroup _progressBarCanvasGroup;

    [SerializeField] private Canvas _interactCanvas;
    [SerializeField] private TextMeshProUGUI _interactText;
    [SerializeField] private Image _progressBar;
    [SerializeField] private TextMeshProUGUI _progressBarText;    

    private void Start()
    {
        Player.Instance.OnTimedInteractableInteraction += OnTimedInteractableInteraction;

        _progressBarCanvasGroup = _progressBar.gameObject.GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        _closestInteractable = Player.Instance.GetClosestInteractable();

        if (MetAllConditionsToShowUI())
        {
            _interactCanvas.gameObject.SetActive(true);
            _interactText.SetText(_closestInteractable.GetInteractText());
        }
        else
        {
            _interactCanvas.gameObject.SetActive(false);
        }
    }

    private bool MetAllConditionsToShowUI()
    {
        return _closestInteractable != null
            && _closestInteractable.MetAllConditionsToInteract()
            && !Player.Instance.InteractingWithUI;
    }

    private void OnTimedInteractableInteraction(object sender, System.EventArgs e)
    {
        _progessBarCoroutine ??= StartCoroutine(ShowProgessBar((sender as ITimedInteractable).GetInteractionTime()));
        _progressBarText.SetText((sender as ITimedInteractable).GetProgessBarText());
    }

    private IEnumerator ShowProgessBar(float duration)
    {
        float fadeDuration = 0.25f;

        yield return FadeProgressBar(1, fadeDuration);
        yield return new WaitForSeconds(duration - fadeDuration);
        yield return FadeProgressBar(0, fadeDuration);

        _progessBarCoroutine = null;
    }

    private IEnumerator FadeProgressBar(float targetAlpha, float fadeDuration)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            _progressBarCanvasGroup.alpha = Mathf.Lerp(_progressBarCanvasGroup.alpha, targetAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _progressBarCanvasGroup.alpha = targetAlpha;
    }
}
