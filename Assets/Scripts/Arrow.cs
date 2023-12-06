using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Transform tip;
    
    private Rigidbody rb;
    private Vector3 lastPosition = Vector3.zero;
    private bool isInAir = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();


        //Stop();
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    public void Fire(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse);
        StartCoroutine(RotateWithVelocity());
        isInAir = true;
        SetPhysics(true);
        StartCoroutine(WaitAndDestroy());
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
            //CheckCollision();
            lastPosition = tip.position;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Linecast(lastPosition, tip.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject.layer != 8)
            {
                Debug.Log($"Arrow collided with {hitInfo.collider.name}");

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
        Debug.Log("calling stop");
    }

    private void SetPhysics(bool usePhysics)
    {
        rb.useGravity = usePhysics;
        rb.isKinematic = !usePhysics;
    }
}
