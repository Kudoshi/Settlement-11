using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class NextButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("References (optional, will auto-find)")]
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Hover Settings")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float hoverSpeed = 0.12f;

    [Header("Click Settings")]
    [SerializeField] private Color pressedColor = Color.red;
    [SerializeField] private float clickScale = 0.96f;
    [SerializeField] private float clickSpeed = 0.08f;

    private Vector3 originalScale;
    private Color originalButtonColor;
    private Color originalTextColor;
    private bool isHovering = false;

    private void Awake()
    {
        // Auto-find references if not assigned
        if (button == null) button = GetComponent<Button>();
        if (buttonImage == null && button != null) buttonImage = button.GetComponent<Image>();
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>(true);

        // Safety: if any required components are missing, warn
        if (button == null) Debug.LogError("[NextButtonEffects] No Button component found on the GameObject.");
        if (buttonImage == null) Debug.LogWarning("[NextButtonEffects] Button Image not found. Color changes will be skipped.");
        if (buttonText == null) Debug.LogWarning("[NextButtonEffects] TextMeshProUGUI not found. Text color changes will be skipped.");

        originalScale = transform.localScale;
        if (buttonImage != null) originalButtonColor = buttonImage.color;
        if (buttonText != null) originalTextColor = buttonText.color;
    }

    private void Start()
    {
        // Make sure this GameObject and the Canvas can receive pointer events
        if (FindObjectOfType<EventSystem>() == null)
            Debug.LogWarning("[NextButtonEffects] No EventSystem found in scene. Add one (GameObject -> UI -> Event System).");

        // If using a Canvas make sure GraphicRaycaster is present (just a hint)
        // (we don't automatically add it here to avoid modifying the scene)
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isHovering = true;
        transform.DOKill();
        transform.DOScale(originalScale * hoverScale, hoverSpeed).SetUpdate(false);
        // Debug for testing
        // Debug.Log("Pointer Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isHovering = false;
        transform.DOKill();
        transform.DOScale(originalScale, hoverSpeed).SetUpdate(false);
        // Debug.Log("Pointer Exit");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        transform.DOKill();
        transform.DOScale(originalScale * clickScale, clickSpeed).SetUpdate(false);

        if (buttonImage != null)
            buttonImage.color = pressedColor;

        if (buttonText != null)
            buttonText.color = pressedColor;

        // Debug.Log("Pointer Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;

        // restore color
        if (buttonImage != null)
            buttonImage.color = originalButtonColor;

        if (buttonText != null)
            buttonText.color = originalTextColor;

        // restore scale to hover OR normal depending if pointer still over
        transform.DOKill();
        if (isHovering)
            transform.DOScale(originalScale * hoverScale, clickSpeed).SetUpdate(false);
        else
            transform.DOScale(originalScale, clickSpeed).SetUpdate(false);

        // Debug.Log("Pointer Up");
    }
}
