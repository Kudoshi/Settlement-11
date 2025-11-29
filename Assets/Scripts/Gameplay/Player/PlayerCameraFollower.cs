
using UnityEngine;

public class PlayerCameraFollower : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}