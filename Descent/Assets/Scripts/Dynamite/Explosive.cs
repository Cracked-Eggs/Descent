using UnityEngine;
using UnityEngine.Events;

public class Explosive : MonoBehaviour
{
    [SerializeField] float _triggerForce = 0.5f;
    [SerializeField] float _explosionRadius = 5;
    [SerializeField] float _explosionForce = 500;
    [SerializeField] float _explosionSpeed = 5;
    [SerializeField] Rigidbody _rb;
    [SerializeField] ParticleSystem _particleSystem;
    AudioManager _audioManager;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = Vector3.forward * _explosionSpeed;
        Instantiate(_particleSystem, transform.position, Quaternion.identity);
        _audioManager = FindObjectOfType<AudioManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= _triggerForce)
        {
            var surroundingObjects = Physics.OverlapSphere(transform.position, _explosionRadius);

            foreach (var obj in surroundingObjects)
            {
                var rb = obj.GetComponent<Rigidbody>();
                if (rb == null) continue;

                rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, 1);
            }
            _audioManager.Play("Explosion");
            Destroy(gameObject);
            _audioManager.Play("Goal");
        }
    }
}