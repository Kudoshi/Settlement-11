using Kudoshi.Utilities;
using System;
using UnityEngine;

public class FireflyOrb : MonoBehaviour
{
    [SerializeField] private float _chaseForce;
    [SerializeField] private int _fireFlyDropAmount;

    private Rigidbody _rb;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        ChasePlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerFirefly>().AdjustFireflies(1);
            Destroy(gameObject);
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = PlayerController.Instance.transform.position - _rb.position;

        _rb.linearVelocity = direction * _chaseForce *Time.deltaTime;
    }
}