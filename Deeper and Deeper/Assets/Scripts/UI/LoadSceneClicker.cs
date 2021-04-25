using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneClicker : MonoBehaviour
{
    public string _nextScene = "Game";

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(new UnityAction(() => SceneManager.LoadScene(_nextScene)));
    }
}
