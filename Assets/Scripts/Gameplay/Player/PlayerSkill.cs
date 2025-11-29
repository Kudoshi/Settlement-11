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
            //// 1. Raycast from camera
            //Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            //RaycastHit hit;

            //Vector3 targetPoint;

            //if (Physics.Raycast(ray, out hit))
            //{
            //    targetPoint = hit.point;
            //}
            //else
            //{
            //    targetPoint = ray.GetPoint(1000);
            //}

            //// 3. Calculate direction toward the target
            //Vector3 direction = targetPoint - weapon.transform.position;

            //GameObject sword_slash = Instantiate(Slash, weapon.transform.position, Quaternion.identity);

            //sword_slash.transform.forward = direction.normalized;

            

            if (PlayerFirefly.Instance.FireflyCounter >= 50)
            {
                animator.SetTrigger("slash");
                PlayerFirefly.Instance.UseAllFireflies();
                Instantiate(Slash, transform.position + transform.forward * 2 + Vector3.up, Quaternion.LookRotation(transform.forward));
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayerFirefly.Instance.AdjustFireflies(10);
        }
    }
}
