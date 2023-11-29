using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] GameObject _replacement;
    [SerializeField] float _breakForce = 2;
    [SerializeField] float _collisionMultiplier = 100;
    [SerializeField] bool _broken;

    void OnCollisionEnter(Collision collision)
    {
        if (_broken) return;
        if (collision.gameObject.CompareTag("Player")) return;
        if (collision.gameObject.CompareTag("Ground")) return;
        if (collision.relativeVelocity.magnitude >= _breakForce)
        {
            _broken = true;
            var replacement = Instantiate(_replacement, transform.position, transform.rotation);

            var rbs = replacement.GetComponentsInChildren<Rigidbody>();

            foreach (var rb in rbs)
                rb.AddExplosionForce(collision.relativeVelocity.magnitude * _collisionMultiplier, collision.contacts[0].point, 2);

            Destroy(gameObject);
            Destroy(replacement, 2f);
        }
    }
}