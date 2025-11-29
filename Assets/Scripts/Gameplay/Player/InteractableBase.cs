
using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class InteractableBase : MonoBehaviour
{
    [SerializeField] private SO_Outline _globalOutlineSetting;
    [SerializeField] private OutlineSetting _outlineSetting;
    [SerializeField] private bool _overwriteOutline;
    private Outline _outline;
    private bool _outlineEnabled = false;
    private void Awake()
    {
        _outline = GetComponent<Outline>();
        SetupOutline();
    }

    private void SetupOutline()
    {
        if (_overwriteOutline)
        {
            _outline.OutlineMode = _outlineSetting.OutlineMode;
            _outline.OutlineColor = _outlineSetting.OutlineColor;
            _outline.OutlineWidth = _outlineSetting.OutlineWidth;

        }
        else
        {
            _outline.OutlineMode = _globalOutlineSetting.GlobalOutlineSetting.OutlineMode;
            _outline.OutlineColor = _globalOutlineSetting.GlobalOutlineSetting.OutlineColor;
            _outline.OutlineWidth = _globalOutlineSetting.GlobalOutlineSetting.OutlineWidth;

        }
    }

    private void Update()
    {
        HighlightObject();
    }

    private void HighlightObject()
    {
        if (_outline.enabled != _outlineEnabled)
        {
            _outline.enabled = _outlineEnabled;
        }
    }

    public void EnableHighlight(bool enableHighlight)
    {
       _outlineEnabled = enableHighlight;
    }

    public abstract void Interact(); 
}