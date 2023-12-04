using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class TimeControllableObject : MonoBehaviour
{
    private bool isManipulatingTime = false;
    private bool timeJustStopped = false;
    public bool hasBeenThrown = false;
    private bool isRewinding = false;
    public float recordTime = 5f;


    List<PointInTime> pointsInTime;

    private Vector3 savedVelocity;
    private Vector3 savedAngularVelocity;
    private Rigidbody rb;

    private void Awake()
    {
        pointsInTime = new List<PointInTime>();
        rb = GetComponent<Rigidbody>();
    }

    public void OnPickedUp()
    {
        hasBeenThrown = false;
        pointsInTime.Clear();
    }

    public void OnReleased()
    {
        hasBeenThrown = true;
        pointsInTime.Clear();
    }

    private void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    private void Rewind()
    {
        if (pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            transform.rotation = pointInTime.rotation;
            pointsInTime.RemoveAt(0);

            StopTime();

        }
        else
        {
            ResumeTime();
        }
    }

    private void Record()
    {
        if (hasBeenThrown || HasSignificantChange())
        {
            if (pointsInTime.Count == 0 || HasSignificantChange())
            {
                pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
            }

            if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
            {
                pointsInTime.RemoveAt(pointsInTime.Count - 1);
            }
        }
    }

    private bool HasSignificantChange()
    {
        float positionThreshold = 0.01f;
        float rotationThreshold = 0.01f;

        if (pointsInTime.Count > 0)
        {
            PointInTime lastPoint = pointsInTime[0];
            float positionDelta = Vector3.Distance(lastPoint.position, transform.position);
            float rotationDelta = Quaternion.Angle(lastPoint.rotation, transform.rotation);

            return positionDelta > positionThreshold || rotationDelta > rotationThreshold;
        }

        return true;
    }

    public void ManipulateTime(float timeControlFactor)
    {
        const float speedUpThreshold = 0.1f;
        const float slowDownThreshold = -0.1f;

        if (timeJustStopped)
        {
            ResumeTime();
            timeJustStopped = false;
        }

        if (timeControlFactor > speedUpThreshold)
        {
            SpeedUpTime(); 
        }
        else if (timeControlFactor < slowDownThreshold && pointsInTime.Count > 0)  
        {
            RewindTime(); 
        }
        else
        {
            if (!isManipulatingTime && !timeJustStopped)
            {
                StopTime();
            }
        }
    }

    private void SpeedUpTime()
    {
        if (rb != null && !isManipulatingTime)
        {
            Debug.Log("Speeding Up Time");

            Vector3 newVelocity = rb.velocity * 2.0f; 
            Vector3 newAngularVelocity = rb.angularVelocity * 2.0f; 


            rb.velocity = newVelocity;
            rb.angularVelocity = newAngularVelocity;

            isManipulatingTime = true;
        }
    }

    private void RewindTime()
    {
        isRewinding = true;
        rb.isKinematic = true;
    }

    public void ResetTimeManipulation()
    {
        isManipulatingTime = false;
        timeJustStopped = false;
        ResumeTime();
    }

    public void StopTime()
    {
        if (rb != null && !timeJustStopped)
        {
            savedVelocity = rb.velocity;
            savedAngularVelocity = rb.angularVelocity;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;


            timeJustStopped = true;
        }
    }


    public void ResumeTime()
    {
        if (rb != null)
        {

            rb.isKinematic = false;
            rb.velocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;

            timeJustStopped = false;
            isRewinding = false;
            isManipulatingTime = false;

        }
    }
}
