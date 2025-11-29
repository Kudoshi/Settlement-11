using UnityEngine;
using UnityEngine.VFX;

public class Bullet : MonoBehaviour
{
    [SerializeField] private SphereCollider _collider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Rigidbody _rb;
    //[SerializeField] private VisualEffect _trailFx;

    private float _bulletDamage;
    private float _bulletSpeed;
    private int _currentReflectCount;
    private bool _activated;

    private void FixedUpdate()
    {
        if (_activated)
        {
            _rb.linearVelocity = transform.forward * _bulletSpeed;
        }
        else if (!_activated && _rb.linearVelocity != Vector3.zero)
        {
            _rb.linearVelocity = Vector3.zero;
        }
    }

    public void StartBullet(float speed, float damage, int reflectCount, float lifetime = 3.0f)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        _bulletDamage = damage;
        _bulletSpeed = speed;
        _currentReflectCount = reflectCount;

        _activated = true;
        _collider.enabled = true;
        _meshRenderer.enabled = true;
        _rb.linearVelocity = Vector3.zero;

        //_trailFx.transform.position = transform.position;
        //_trailFx.Play();

        Invoke("ResetBullet", lifetime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            SanityManager.Instance.DecreaseSanity(_bulletDamage);
        
            ResetBullet();

        }

    }


    private void ResetBullet()
    {
        Destroy(gameObject);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartBullet(_bulletSpeed, _bulletDamage, _currentReflectCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
