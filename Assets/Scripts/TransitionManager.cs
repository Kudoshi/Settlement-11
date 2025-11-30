using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private GameObject transitionCanvasPrefab;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Ease fadeEase = Ease.InOutQuad;

    private CanvasGroup fadeCanvasGroup;
    private Image fadeImage;
    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (transitionCanvasPrefab != null)
            InstantiatePrefab();
        else
            CreateFadeUI();
    }

    private void InstantiatePrefab()
    {
        GameObject canvasObj = Instantiate(transitionCanvasPrefab, transform);
        canvas = canvasObj.GetComponent<Canvas>();

        Transform fadePanelTransform = canvasObj.transform.Find("FadePanel");
        if (fadePanelTransform != null)
        {
            fadeCanvasGroup = fadePanelTransform.GetComponent<CanvasGroup>();
            fadeImage = fadePanelTransform.GetComponent<Image>();

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 1f;
                fadeCanvasGroup.blocksRaycasts = true;
            }

            if (fadeImage != null)
                fadeImage.color = fadeColor;
        }
    }

    private void CreateFadeUI()
    {
        GameObject canvasObj = new GameObject("TransitionCanvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasObj.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject fadePanel = new GameObject("FadePanel");
        fadePanel.transform.SetParent(canvasObj.transform, false);

        RectTransform rect = fadePanel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        fadeImage = fadePanel.AddComponent<Image>();
        fadeImage.color = fadeColor;

        fadeCanvasGroup = fadePanel.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;
    }

    public void FadeIn(Action onComplete = null)
    {
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.DOFade(1f, fadeDuration).SetEase(fadeEase).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void FadeOut(Action onComplete = null)
    {
        fadeCanvasGroup.DOFade(0f, fadeDuration).SetEase(fadeEase).OnComplete(() =>
        {
            fadeCanvasGroup.blocksRaycasts = false;
            onComplete?.Invoke();
        });
    }

    public void FadeInOut(Action onFadedIn = null)
    {
        FadeIn(() =>
        {
            onFadedIn?.Invoke();
            FadeOut();
        });
    }

    public void SetFadeColor(Color color)
    {
        if (fadeImage != null)
            fadeImage.color = color;
    }

    public void SetFadeDuration(float duration)
    {
        fadeDuration = duration;
    }
}
