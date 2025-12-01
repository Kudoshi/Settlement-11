using Kudoshi.Utilities;
using System;
using UnityEngine;

public class FireflyOrb : MonoBehaviour
{
    [SerializeField] private float _chaseForce;
    [SerializeField] private int _fireFlyDropAmount;
    [SerializeField] private float _dieAfterSecond = 5.0f;

    private Rigidbody _rb;
    private bool _enableFly;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Util.WaitForSeconds(this, () => _enableFly = true, 0.5f);
        Util.WaitForSeconds(this, () => Destroy(gameObject), _dieAfterSecond);
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
            SoundManager.Instance.PlaySound("sfx_firefly_gain");
            Destroy(gameObject);
        }
    }

    private void ChasePlayer()
    {
        if (!_enableFly) return;
        Vector3 direction = PlayerCamera.Instance.transform.position - _rb.position + new Vector3(0,-.5f,0);

        _rb.linearVelocity = direction * _chaseForce *Time.deltaTime;
        transform.LookAt(PlayerCamera.Instance.transform);
    }
}