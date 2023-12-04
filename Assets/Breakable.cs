using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject _replacement;
    [SerializeField] private float breakForce = 2f;
    [SerializeField] private float collisionMultipier = 100;
    [SerializeField] private bool broken;

    private void OnCollisionEnter(Collision collision)
    {
        if (broken) return;
        if (collision.relativeVelocity.magnitude > breakForce)
        {
            broken = true;
            var replacement = Instantiate(_replacement, transform.position, transform.rotation);

            var rbs = replacement.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.AddExplosionForce(collision.relativeVelocity.magnitude * collisionMultipier, collision.contacts[0].point, 2);
            }
            Destroy(gameObject);
        }
    }
}
