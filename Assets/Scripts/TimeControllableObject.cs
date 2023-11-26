using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class TimeControllableObject : MonoBehaviour
{
    private bool isManipulatingTime = false;
    private bool timeJustStopped = false;
    private bool hasBeenThrown = false;
    private bool isTimeStopped = false;
    private bool isRewinding = false;
    public float recordTime = 2f;


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
        if (hasBeenThrown)
        {
            if(pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
            {
                pointsInTime.RemoveAt(pointsInTime.Count - 1);
            }
                
            pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
        }
    }

    public void ManipulateTime(float joyStickX)
    {
        const float localStopTimeThreshold = 0.3f;

        if (Mathf.Abs(joyStickX) < localStopTimeThreshold)
        {
            if (!isManipulatingTime && !timeJustStopped)
            {
                // Stop time if not already manipulating time and time isn't already stopped
                StopTime();
            }
        }
        else // If the rotation factor is outside the threshold
        {
            if (timeJustStopped)
            {
                ResumeTime();
                timeJustStopped = false;
            }

            // Check if joystick is moved to the right to fast-forward time
            if (joyStickX > localStopTimeThreshold)
            {
                Debug.Log("Calling SpeedUpTime");
                SpeedUpTime(joyStickX - localStopTimeThreshold); // Adjust factor based on how far joystick is moved
                //Debug.Log("FastForward Time");
            }
            else if (joyStickX < -localStopTimeThreshold)
            {
                Debug.Log("Calling SlowDownTime");
                if(pointsInTime.Count > 0)
                {
                    RewindTime(-(joyStickX + localStopTimeThreshold)); // Adjust factor based on how far joystick is moved
                }
                //Debug.Log("Reqinding Time");
            }
        }
    }

    private void SpeedUpTime(float factor)
    {
        if (rb != null && !isManipulatingTime)
        {

            Debug.Log("Speeding Up Time");
            // Adjust the Rigidbody's properties to speed up the object
            float speedMultiplier = 1 + factor; // This will speed up the object based on the joystick input
            rb.velocity *= speedMultiplier;
            rb.angularVelocity *= speedMultiplier;

            // Optionally, you could also modify other properties like drag to simulate different time speeds
            // For example, reducing drag could simulate a 'faster' environment

            isManipulatingTime = true; // Indicate that we are now manipulating time
        }
    }

    private void RewindTime(float factor)
    {
        isRewinding = true;
        rb.isKinematic = true;
    }

    public void ResetTimeManipulation()
    {
        isManipulatingTime = false;
        timeJustStopped = false;
        ResumeTime(); // Resume normal time flow if it was being manipulated
    }

    public void StopTime()
    {
        if (rb != null && !timeJustStopped)
        {
            // Save the current velocities
            savedVelocity = rb.velocity;
            savedAngularVelocity = rb.angularVelocity;

            // Set the velocities to zero to stop the object
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Make the Rigidbody kinematic
            rb.isKinematic = true;

            // Indicate that time has just stopped and recording should pause
            isTimeStopped = true;
            timeJustStopped = true;
        }
    }


    public void ResumeTime()
    {
        if (rb != null)
        {
            // Restore the object's velocities
            rb.isKinematic = false;
            rb.velocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;

            // Reset the flags
            isTimeStopped = false;
            timeJustStopped = false;
            isRewinding = false;
            isManipulatingTime = false;

        }
    }
}
