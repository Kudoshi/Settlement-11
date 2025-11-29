
using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class InteractableBase : MonoBehaviour
{
    private Outline _outline;
    private bool _outlineEnabled = false;
    private void Awake()
    {
        _outline = GetComponent<Outline>();
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