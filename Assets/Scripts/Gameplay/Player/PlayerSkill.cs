using DG.Tweening.Core.Easing;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public GameObject Slash;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (PlayerFirefly.Instance.UseAllFireflies())
            {
                // trigger animation here...
                Instantiate(Slash, Camera.main.transform.position + Camera.main.transform.forward * 2 + Vector3.up, Quaternion.LookRotation(transform.forward));
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
