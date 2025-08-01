using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CloseButton : MonoBehaviour
{
    private Button _closeButton;

    [SerializeField] private GameObject _screenToClose;

    private void Awake()
    {
        _closeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(() => CloseUpgradeUI());
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveAllListeners();
    }

    private void CloseUpgradeUI()
    {
        _screenToClose.SetActive(false);
        Player.Instance.LockCursor();
    }
}
