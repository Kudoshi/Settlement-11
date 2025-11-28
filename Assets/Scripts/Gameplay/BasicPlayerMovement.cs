using System.Globalization;
using UnityEngine;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem;
#endif

/// <summary>
/// A basic example of client authoritative movement. It works in both client-server and distributed-authority scenarios.
/// If you want to modify this Script please copy it into your own project and add it to your Player Prefab.
/// </summary>
public class BasicPlayerMovement : MonoBehaviour
{
    public float Speed = 5;

    public float Rotate = 150f;

    void Update()
    {

        var multiplier = Speed * Time.deltaTime;
        var rotateMultiplier = Rotate * Time.deltaTime;

        // Old input backends are enabled.
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-multiplier, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(multiplier, 0, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, multiplier);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, 0, -multiplier);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(new Vector3(0, -rotateMultiplier));
        }
        if (Input.GetKey(KeyCode.X))
        {
            transform.Rotate(new Vector3(0, rotateMultiplier));
        }
    }
}
