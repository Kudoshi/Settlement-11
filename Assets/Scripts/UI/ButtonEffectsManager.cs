using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class ButtonEffectsManager : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private float hoverSlideDistance = 10f;
    [SerializeField] private float hoverSpeed = 0.15f;
    [SerializeField] private float clickScale = 0.95f;
    [SerializeField] private float clickSpeed = 0.1f;

    private Dictionary<Button, Vector2> originalPositions = new Dictionary<Button, Vector2>();
    private Dictionary<Button, Vector3> originalScales = new Dictionary<Button, Vector3>();

    private void Start()
    {
        foreach (var button in buttons)
        {
            if (button == null) continue;

            RectTransform rectTransform = button.GetComponent<RectTransform>();
            originalPositions[button] = rectTransform.anchoredPosition;
            originalScales[button] = button.transform.localScale;

            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

            AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) => OnHoverEnter(button));
            AddEventTrigger(trigger, EventTriggerType.PointerExit, (data) => OnHoverExit(button));
            AddEventTrigger(trigger, EventTriggerType.PointerDown, (data) => OnClickDown(button));
            AddEventTrigger(trigger, EventTriggerType.PointerUp, (data) => OnClickUp(button));
        }
    }

    private void OnHoverEnter(Button button)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.DOKill();
        rectTransform
        .DOAnchorPos(originalPositions[button] + Vector2.left * hoverSlideDistance, hoverSpeed)
        .SetUpdate(true);
        //rectTransform.DOAnchorPos(originalPositions[button] + Vector2.left * hoverSlideDistance, hoverSpeed);
    }

    private void OnHoverExit(Button button)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.DOKill();
        rectTransform
        .DOAnchorPos(originalPositions[button], hoverSpeed)
        .SetUpdate(true);
        //rectTransform.DOAnchorPos(originalPositions[button], hoverSpeed);
    }

    private void OnClickDown(Button button)
    {
        button.transform.DOKill();
        button.transform.DOScale(originalScales[button] * clickScale, clickSpeed).SetUpdate(true);
    }

    private void OnClickUp(Button button)
    {
        button.transform.DOKill();
        button.transform.DOScale(originalScales[button], clickSpeed).SetUpdate(true);
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((data) => callback(data));
        trigger.triggers.Add(entry);
    }

}
