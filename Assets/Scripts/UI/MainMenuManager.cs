using UnityEngine;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private RectTransform creditsPanel;
    [SerializeField] private RectTransform optionsPanel;

    [SerializeField] private float slideInSpeed = 0.5f;
    [SerializeField] private float slideOutSpeed = 0.4f;
    [SerializeField] private Ease slideInEase = Ease.OutBack;
    [SerializeField] private Ease slideOutEase = Ease.InBack;
    [SerializeField] private float punchScale = 0.05f;

    private void Start()
    {
        mainPanel.anchoredPosition = new Vector2(0f, mainPanel.anchoredPosition.y);
        creditsPanel.anchoredPosition = new Vector2(2000f, creditsPanel.anchoredPosition.y);
        optionsPanel.anchoredPosition = new Vector2(2000f, optionsPanel.anchoredPosition.y);
        TransitionManager.Instance.FadeOut();
    }

    public void StartGame()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(1);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void OpenOptions()
    {
        mainPanel.DOAnchorPosX(-180f, slideOutSpeed).SetEase(slideOutEase);
        optionsPanel.DOAnchorPosX(0f, slideInSpeed).SetEase(slideInEase)
            .OnComplete(() => optionsPanel.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f));
    }

    public void OpenCredits()
    {
        mainPanel.DOAnchorPosX(-200f, slideOutSpeed).SetEase(slideOutEase);
        creditsPanel.DOAnchorPosX(0f, slideInSpeed).SetEase(slideInEase)
            .OnComplete(() => creditsPanel.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f));
    }

    public void GoToMain()
    {
        mainPanel.DOAnchorPosX(0f, slideInSpeed).SetEase(slideInEase)
            .OnComplete(() => mainPanel.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f));
        creditsPanel.DOAnchorPosX(1000f, slideOutSpeed).SetEase(slideOutEase);
        optionsPanel.DOAnchorPosX(1000f, slideOutSpeed).SetEase(slideOutEase);
    }

    public void Quit()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.QuitGame();
        else
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
