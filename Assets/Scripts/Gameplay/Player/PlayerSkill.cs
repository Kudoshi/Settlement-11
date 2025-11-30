using DG.Tweening.Core.Easing;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public GameObject Slash;
    private Vector3 pos;

    private void Start()
    {
        pos = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y - 5f, Camera.main.transform.position.z);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (PlayerFirefly.Instance.UseAllFireflies())
            {
                // trigger animation here...
                Instantiate(Slash, Camera.main.transform.position + Camera.main.transform.forward * 2, Quaternion.LookRotation(transform.forward));
            }
            Debug.Log("not enough fireflies");
        }

        // For debug purpose
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayerFirefly.Instance.AdjustFireflies(1);
        }
    }
}
