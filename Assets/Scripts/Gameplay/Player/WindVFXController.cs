
using UnityEngine;

public class WindVFXController : MonoBehaviour
{
    [SerializeField] private Vector3 _offsetPosition;
    [SerializeField] private Vector3 _offsetRotation;
    private void Start()
    {
        transform.parent = PlayerCamera.Instance.transform;
        transform.position = PlayerCamera.Instance.transform.position + _offsetPosition;
        transform.rotation = PlayerCamera.Instance.transform.rotation * Quaternion.Euler(_offsetRotation);
    }
}