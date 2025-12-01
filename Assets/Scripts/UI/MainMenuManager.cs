using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private RectTransform creditsPanel;
    [SerializeField] private RectTransform optionsPanel;

    [Header("Slide Settings")]
    [SerializeField] private float slideInSpeed = 0.5f;
    [SerializeField] private float slideOutSpeed = 0.4f;
    [SerializeField] private Ease slideInEase = Ease.OutBack;
    [SerializeField] private Ease slideOutEase = Ease.InBack;
    [SerializeField] private float punchScale = 0.05f;

    private float centerX = 0f;
    private float offscreenRight;
    private float offscreenLeft;

    private void Start()
    {
        // Calculate off-screen distances based on screen width
        offscreenRight = Screen.width * 1.2f;
        offscreenLeft = -Screen.width * 1.2f;

        // Initial positions
        mainPanel.anchoredPosition = new Vector2(centerX, 0f);
        creditsPanel.anchoredPosition = new Vector2(offscreenRight, 0f);
        optionsPanel.anchoredPosition = new Vector2(offscreenRight, 0f);

        TransitionManager.Instance.FadeOut();
    }

    public void OpenOptions()
    {
        mainPanel.DOAnchorPosX(offscreenLeft, slideOutSpeed).SetEase(slideOutEase);

        optionsPanel.DOAnchorPosX(centerX, slideInSpeed)
            .SetEase(slideInEase)
            .OnComplete(() =>
                optionsPanel.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f));
    }

    public void OpenCredits()
    {
        mainPanel.DOAnchorPosX(offscreenLeft, slideOutSpeed).SetEase(slideOutEase);

        creditsPanel.DOAnchorPosX(centerX, slideInSpeed)
            .SetEase(slideInEase)
            .OnComplete(() =>
                creditsPanel.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f));
    }

    public void GoToMain()
    {
        mainPanel.DOAnchorPosX(centerX, slideInSpeed)
            .SetEase(slideInEase)
            .OnComplete(() =>
                mainPanel.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f));

        creditsPanel.DOAnchorPosX(offscreenRight, slideOutSpeed).SetEase(slideOutEase);
        optionsPanel.DOAnchorPosX(offscreenRight, slideOutSpeed).SetEase(slideOutEase);
    }


    public void OnPlayClicked()
    {
        SceneManager.LoadScene("Room");
    }
}
