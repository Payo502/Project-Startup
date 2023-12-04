using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public Transform tip;

    private Rigidbody rb;
    private bool isInAir = false;
    private Vector3 lastPosition = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PullInteraction.PullActionReleased += Release;


        Stop();
    }

    private void OnDestroy()
    {
        PullInteraction.PullActionReleased -= Release;
        //PullInteraction.PullActionReleased -= Fire;
    }

    private void Release(float value)
    {
        PullInteraction.PullActionReleased -= Release;
        gameObject.transform.parent = null;
        isInAir = true;
        SetPhysics(true);

        Vector3 force = transform.forward * value * speed;
        rb.AddForce(force, ForceMode.Impulse);

        StartCoroutine(RotateWithVelocity());

        lastPosition = tip.position;

    }

    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (isInAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(rb.velocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if (isInAir)
        {
            CheckCollision();
            lastPosition = tip.position;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Linecast(lastPosition, tip.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject.layer != 8)
            {
                if (hitInfo.transform.TryGetComponent(out Rigidbody body))
                {
                    rb.interpolation = RigidbodyInterpolation.None;
                    transform.parent = hitInfo.transform;
                    body.AddForce(rb.velocity, ForceMode.Impulse);
                }
                Stop();
            }
        }
    }

    private void Stop()
    {
        isInAir = false;
        SetPhysics(false);
    }

    private void SetPhysics(bool usePhysics)
    {
        rb.useGravity = usePhysics;
        rb.isKinematic = !usePhysics;
    }
}
