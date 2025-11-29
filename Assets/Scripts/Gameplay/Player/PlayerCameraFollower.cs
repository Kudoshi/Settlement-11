
using Kudoshi.Utilities;
using UnityEngine;

public class PlayerCameraFollower : Singleton<PlayerCameraFollower>
{
    public Transform HeadOrientation;
    public Transform Head;
    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}