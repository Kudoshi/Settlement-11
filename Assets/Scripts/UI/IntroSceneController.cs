using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "NewMainMenu";

    private void Start()
    {
        // Start video after 2 seconds
        Invoke(nameof(StartVideo), 2f);

        // Swap scene after 8 seconds total
        Invoke(nameof(LoadNextScene), 8f);
    }

    private void StartVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
