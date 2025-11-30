using UnityEngine;

public class KeepParticleUpright : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform != null)
        {
            transform.rotation = cameraTransform.rotation;
        }
    }
}
