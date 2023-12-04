using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float triggerForce = 0.5f;
    [SerializeField] private float explosionRadius = 5;
    [SerializeField] private float explosionForce = 500;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= triggerForce)
        {
            var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (var obj in surroundingObjects)
            {
                var rb = obj.GetComponent<Rigidbody>();
                if (rb == null) continue;

                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            Destroy(gameObject);
        }   
    }
}
