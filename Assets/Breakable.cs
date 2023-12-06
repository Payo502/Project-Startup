using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Breakable : MonoBehaviour
{
    [SerializeField] private GameObject _replacement;
    [SerializeField] private GameObject floor;
    [SerializeField] private float breakForce = 2f;
    [SerializeField] private float collisionMultipier = 100;
    [SerializeField] private bool broken;
    [SerializeField] private Transform parentTransform;
    private VisualEffect visualEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (broken) return;
        if (collision.relativeVelocity.magnitude > breakForce)
        {
            Debug.Log("Statue breaking due to collision with force: " + collision.relativeVelocity.magnitude);

            broken = true;
            Instantiate(_replacement, parentTransform.transform.position, transform.rotation);

            /*            //var rbs = replacement.GetComponentsInChildren<Rigidbody>();
                        foreach (var rb in rbs)
                        {
                            //rb.AddExplosionForce(collision.relativeVelocity.magnitude * collisionMultipier, collision.contacts[0].point, 2);
                        }*/

            Debug.Log("Statue fragments breaking with a force of: " + collision.relativeVelocity.magnitude * collisionMultipier);
            
            if (floor != null)
            {
                Destroy(floor);
            }

            Destroy(gameObject);
        }
    }

/*    void Update()
    {
        if (broken)
        {
            Instantiate(_replacement, parentTransform.transform.position, transform.rotation);
            Destroy(gameObject);

        }
    }*/
}
