using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPoint : MonoBehaviour
{
    [SerializeField] private float forceFactor = 20000f;
    [SerializeField] private float damping = 0.1f;
    [SerializeField] private float maxVelocity = 5f;

    private SphereCollider magneticField;
    List<Rigidbody> rgBalls = new List<Rigidbody>();

    Transform magnetP;

    void Awake()
    {
        magnetP = GetComponent<Transform>();
        magneticField = GetComponent<SphereCollider>();
    }

    private void FixedUpdate()
    {
        foreach (Rigidbody rgBall in rgBalls)
        {
            float distance = Vector2.Distance(transform.position, rgBall.position);
            if (distance <= magneticField.radius)
            {
                Vector3 direction = (transform.position - rgBall.position).normalized;
                float forceMagnitude = Mathf.Clamp(forceFactor / (distance * distance), 0f, forceFactor);
                rgBall.AddForce(direction * forceMagnitude * Time.fixedDeltaTime);

                rgBall.velocity *= 1 - damping;

                if (rgBall.velocity.magnitude > maxVelocity)
                {
                    rgBall.velocity = rgBall.velocity.normalized * maxVelocity;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            rgBalls.Add(other.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            rgBalls.Remove(other.GetComponent<Rigidbody>());
        }
    }
}
