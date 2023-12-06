using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform tip;
    [SerializeField] private LayerMask ignoreLayerMask;

    private Rigidbody rb;
    private bool isInAir = false;
    private Vector3 lastPosition = Vector3.zero;

    private ParticleSystem particleSystem;
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PullInteraction.PullActionReleased += Release;

        particleSystem = GetComponentInChildren<ParticleSystem>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();


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

        particleSystem.Play();
        trailRenderer.emitting = true;

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
        if (Physics.Linecast(lastPosition, tip.position, out RaycastHit hitInfo, ~ignoreLayerMask))
        {
            if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("Invisible Wall"))
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

        //particleSystem.Stop();
        trailRenderer.emitting = false;
    }

    private void SetPhysics(bool usePhysics)
    {
        rb.useGravity = usePhysics;
        rb.isKinematic = !usePhysics;
    }
}
