using Kudoshi.Utilities;
using System;
using System.Collections;
using UnityEngine;

public class FireflyOrb : MonoBehaviour
{
    [SerializeField] private float _chaseForce = 20f;
    [SerializeField] private int _fireFlyDropAmount;
    [SerializeField] private float _dieAfterSecond = 5.0f;
    [SerializeField] private float _startDelay = 0.5f;
    [SerializeField] private float _initialUpForce = 5f;

    private Rigidbody _rb;
    private bool _canChase = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Util.WaitForSeconds(this, () => Destroy(gameObject), _dieAfterSecond);
    
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(-0.5f, 0.5f));
        _rb.linearVelocity = (Vector3.up + randomOffset).normalized * _initialUpForce;
        StartCoroutine(StartChaseRoutine());
    }

    private IEnumerator StartChaseRoutine()
    {
        yield return new WaitForSeconds(_startDelay);
        _canChase = true;
    }

    private void Update()
    {
        if (_canChase)
        {
            ChasePlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound("sfx_firefly_gain");
            other.GetComponent<PlayerFirefly>().AdjustFireflies(_fireFlyDropAmount);
            Destroy(gameObject);
        }
    }

    private void ChasePlayer()
    {
        transform.LookAt(PlayerCamera.Instance.transform);
        if (PlayerController.Instance == null) return;

        Vector3 direction = (PlayerController.Instance.transform.position + Vector3.up) - transform.position;
        direction.Normalize();

        // Smoothly turn towards the player
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, direction * _chaseForce, Time.deltaTime * 5f);
    }
}