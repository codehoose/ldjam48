using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuitGameClick : MonoBehaviour
{
    public CanvasGroup _quitPanel;

    public Button _yes;
    public Button _no;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
        {
            _quitPanel.blocksRaycasts = true;
            _quitPanel.alpha = 1;
        }));

        _yes.onClick.AddListener(new UnityAction(() =>
        {
            Application.Quit();
        }));

        _no.onClick.AddListener(new UnityAction(() =>
        {
            _quitPanel.blocksRaycasts = false;
            _quitPanel.alpha = 0;
        }));
    }
}
