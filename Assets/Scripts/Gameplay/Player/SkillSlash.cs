using System;
using System.Collections;
using UnityEngine;

public class SkillSlash : MonoBehaviour
{
    private Transform camTrans;

    private void Start()
    {
        camTrans = Camera.main.transform;
        //transform.Rotate(new Vector3(-20, 0, 0), Space.Self);
    }

    void Update()
    {
        gameObject.transform.Translate(camTrans.forward * Time.deltaTime * 20);
        StartCoroutine(DestroyAfterTime(1f));
    }

    IEnumerator DestroyAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Skill Slash hit enemy");
            Destroy(gameObject);
        }
    }
}
