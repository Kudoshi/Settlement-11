using Kudoshi.Utilities;
using System;
using UnityEngine;

public class PlayerInteractable : Singleton<PlayerInteractable>
{
    [SerializeField] private float _interactDistance;
    [SerializeField] private LayerMask _interactLayers;

    private InteractableBase _highlightedInteractable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        Raycast();
    }

    private void Raycast()
    {
        Transform camTrans = Camera.main.transform;
        Ray ray = new Ray(camTrans.position, camTrans.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactLayers))
        {
            if (hit.collider.TryGetComponent<InteractableBase>(out InteractableBase interactable))
            {
                if (_highlightedInteractable != null && _highlightedInteractable != interactable)
                {
                    _highlightedInteractable.EnableHighlight(false);
                }

                _highlightedInteractable = interactable;
                interactable.EnableHighlight(true);
            }
        }
        else
        {
            if (_highlightedInteractable != null)
            {
                _highlightedInteractable.EnableHighlight(false);
                _highlightedInteractable = null;
            }
        }
    }

    private void Interact()
    {
        if (_highlightedInteractable != null)
        {
            _highlightedInteractable.Interact();
        }
    }
    private void OnDrawGizmos()
    {
        Transform camTrans = Camera.main.transform;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(camTrans.position, camTrans.position + camTrans.forward * _interactDistance);
    }

}
