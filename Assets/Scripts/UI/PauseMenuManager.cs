using DG.Tweening;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform pausePanel;
    [SerializeField] private RectTransform optionsPanel;

    [SerializeField] private float slideInSpeed = 0.5f;
    [SerializeField] private float slideOutSpeed = 0.4f;
    [SerializeField] private Ease slideInEase = Ease.OutBack;
    [SerializeField] private Ease slideOutEase = Ease.InBack;
    [SerializeField] private float punchScale = 0.05f;

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(false);
        TransitionManager.Instance.FadeOut();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0f; // to pause game

        pausePanel.anchoredPosition = new Vector2(-800f, pausePanel.anchoredPosition.y);
        pausePanel.DOAnchorPosX(0f, slideInSpeed)
            .SetEase(slideInEase)
            .SetUpdate(true) 
            .OnComplete(() => pausePanel.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f).SetUpdate(true));
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.DOAnchorPosX(-800f, slideOutSpeed)
            .SetEase(slideOutEase)
            .SetUpdate(true)
            .OnComplete(() => pausePanel.gameObject.SetActive(false));
        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        optionsPanel.gameObject.SetActive(true);

        pausePanel.DOAnchorPosX(-200f, slideOutSpeed)
            .SetEase(slideOutEase)
            .SetUpdate(true);

        optionsPanel.DOAnchorPosX(0f, slideInSpeed)
            .SetEase(slideInEase)
            .SetUpdate(true)
            .OnComplete(() => optionsPanel.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f).SetUpdate(true));
    }

    public void BackToPause()
    {
        pausePanel.gameObject.SetActive(true);
        pausePanel.DOAnchorPosX(0f, slideInSpeed)
            .SetEase(slideInEase)
            .SetUpdate(true)
            .OnComplete(() => pausePanel.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f).SetUpdate(true));

        optionsPanel.DOAnchorPosX(640f, slideOutSpeed)
            .SetEase(slideOutEase)
            .SetUpdate(true)
            .OnComplete(() => optionsPanel.gameObject.SetActive(false));
    }

    public void GoBackMainMenu()
    {
        Time.timeScale = 1f;
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene("MainMenu");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
