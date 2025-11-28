using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(int sceneIndex, bool useFade = true)
    {
        if (useFade && TransitionManager.Instance != null)
        {
            TransitionManager.Instance.FadeIn(() =>
            {
                SceneManager.LoadScene(sceneIndex);
                TransitionManager.Instance.FadeOut();
            });
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    public void LoadScene(string sceneName, bool useFade = true)
    {
        if (useFade && TransitionManager.Instance != null)
        {
            TransitionManager.Instance.FadeIn(() =>
            {
                SceneManager.LoadScene(sceneName);
                TransitionManager.Instance.FadeOut();
            });
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void ReloadCurrentScene(bool useFade = true)
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex, useFade);
    }

    public void QuitGame()
    {
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.FadeIn(() =>
            {
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            });
        }
        else
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
