using Kudoshi.Utilities;
using System;
using UnityEngine;

public class FireflyOrb : MonoBehaviour
{
    [SerializeField] private float _chaseForce;

    private Rigidbody _rb;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        Vector3 direction = PlayerController.Instance.transform.position - _rb.position;

        _rb.linearVelocity = direction * _chaseForce *Time.deltaTime;
    }
}