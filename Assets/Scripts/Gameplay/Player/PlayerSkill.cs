using DG.Tweening.Core.Easing;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public float offsetDistance = 2f;     
    public Vector3 spawnOffset = Vector3.up;  
    public GameObject Slash;
    public GameObject weapon;
    public float slashForce = 100f;
    public Animator animator; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerFirefly.Instance.FireflyCounter >= 50)
            {
                animator.SetTrigger("slash");
                PlayerFirefly.Instance.UseAllFireflies();
                Instantiate(Slash, Camera.main.transform.position + Camera.main.transform.forward * 2 + Vector3.up, Quaternion.LookRotation(transform.forward));
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayerFirefly.Instance.AdjustFireflies(10);
        }
    }
}
