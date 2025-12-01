using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class OutroManager : MonoBehaviour
{
    [Header("Outro Panels (Left → Right Order)")]
    [SerializeField] private RectTransform[] images;

    [Header("Next Button (Text will change to Finish)")]
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI nextButtonText;

    [Header("Animation Settings")]
    [SerializeField] private float slideInSpeed = 0.5f;
    [SerializeField] private float slideOutSpeed = 0.4f;
    [SerializeField] private Ease slideInEase = Ease.OutBack;
    [SerializeField] private Ease slideOutEase = Ease.InBack;
    [SerializeField] private float punchScale = 0.05f;

    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "NewMainMenu";
    [SerializeField] private float finalFadeDuration = 1f;

    private int currentIndex = 0;
    private float centerX = 0f;
    private float offscreenRight;
    private float offscreenLeft;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        offscreenRight = Screen.width * 1.2f;
        offscreenLeft = -Screen.width * 1.2f;

        // Position all images: first visible, the rest offscreen
        for (int i = 0; i < images.Length; i++)
        {
            if (i == 0)
                images[i].anchoredPosition = new Vector2(centerX, 0f);
            else
                images[i].anchoredPosition = new Vector2(offscreenRight, 0f);

            SetAlpha(images[i], 1f);
        }

        // First image bounce
        if (images.Length > 0)
            images[0].DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f);
    }

    public void NextImage()
    {
        // Not last image → slide to next
        if (currentIndex < images.Length - 1)
        {
            SlideToNext();

            // If we've just moved TO the last image → update button text
            if (currentIndex == images.Length - 1)
                nextButtonText.text = "Finish";
        }
        else
        {
            // Last image → fade and exit
            FadeLastImageAndExit();
        }
    }

    private void SlideToNext()
    {
        RectTransform current = images[currentIndex];
        RectTransform next = images[currentIndex + 1];

        current.DOAnchorPosX(offscreenLeft, slideOutSpeed)
               .SetEase(slideOutEase);

        next.DOAnchorPosX(centerX, slideInSpeed)
            .SetEase(slideInEase)
            .OnComplete(() =>
                next.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 0.5f));

        currentIndex++;
    }

    private void FadeLastImageAndExit()
    {
        // Disable the next button immediately to prevent double clicks
        if (nextButton != null)
            nextButton.interactable = false;

        RectTransform last = images[currentIndex];

        // Build a sequence to fade last image + button image + TMP text together
        Sequence seq = DOTween.Sequence();

        // Fade the last image (Image component) OR the CanvasGroup if used
        Image img = last.GetComponent<Image>();
        CanvasGroup cg = last.GetComponent<CanvasGroup>();

        if (img != null)
        {
            seq.Join(img.DOFade(0f, finalFadeDuration));
        }
        else
        {
            if (cg == null)
                cg = last.gameObject.AddComponent<CanvasGroup>();

            seq.Join(cg.DOFade(0f, finalFadeDuration));
        }

        // Fade the button's Image (background)
        if (nextButton != null)
        {
            Image btnImg = nextButton.GetComponent<Image>();
            if (btnImg != null)
            {
                seq.Join(btnImg.DOFade(0f, finalFadeDuration));
            }
        }

        // Fade the TextMeshProUGUI text
        if (nextButtonText != null)
        {
            // DOTween supports DOFade on TextMeshProUGUI
            seq.Join(nextButtonText.DOFade(0f, finalFadeDuration));
        }

        // When the whole sequence completes, load next scene
        seq.OnComplete(() => SceneManager.LoadScene(nextSceneName));
    }

    private void SetAlpha(RectTransform t, float alpha)
    {
        Image img = t.GetComponent<Image>();
        CanvasGroup cg = t.GetComponent<CanvasGroup>();

        if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
        else
        {
            if (cg == null)
                cg = t.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = alpha;
        }
    }
}
