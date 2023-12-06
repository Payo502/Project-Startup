using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningBall : MonoBehaviour
{
    [SerializeField] private float threshold;
    [SerializeField] private Transform respawnPoint;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (transform.position.y < threshold)
        {
            transform.position = respawnPoint.position;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
