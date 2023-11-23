using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class TimeControllableObject : MonoBehaviour
{
    private bool isRewinding = false;
    private bool isTimeStopped = false;
    private bool isTimeSpeedingUp = false;

    private Stack<Vector4> positionHistory;

    private float speedUpMultiplier = 2f;
    private float recordTimeStep = 0.02f;

    private Vector3 savedVelocity;
    private Vector3 savedAngularVelocity;

    private void Awake()
    {
        positionHistory = new Stack<Vector4>();
    }

    private void Update()
    {
        if (isTimeStopped) return;

        if (isRewinding)
        {
            Rewind();
        }else if (isTimeSpeedingUp)
        {
            SpeedUpTime();
        }
        else
        {
            RecordPosition();
        }
    }

    public void StartRewind()
    {
        if (!isTimeStopped)
        {
            isRewinding = true;
            isTimeSpeedingUp = false;
        }
    }

    public void StopRewind()
    {
        isRewinding = false;
    }

    public void StartTimeStop()
    {
        isTimeStopped = true ;
        isRewinding = false;
        isTimeSpeedingUp = false;
    }

    public void StopTimeStop()
    {
        isTimeStopped = false;
    }

    public void StartTimeSpeedUp()
    {
        if (!isTimeStopped)
        {
            isTimeSpeedingUp = true;
            isRewinding = false;
        }
    }

    public void StopTimeSpeedUp()
    {
        isTimeSpeedingUp = false;
    }

    public void StopTime()
    {
        Debug.Log("Stopping Time");
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Save the current velocities
            savedVelocity = rb.velocity;
            savedAngularVelocity = rb.angularVelocity;

            // Set the velocities to zero to stop the object
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Optionally, if you want to ensure the object does not move or react to forces
            rb.isKinematic = true;
        }
    }

    public void ResumeTime()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Restore the object's velocities to continue its motion
            rb.velocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;

            // If you made the Rigidbody kinematic in StopTime(), make sure to revert that change
            rb.isKinematic = false;
        }
    }

    public void SpeedUpTime()
    {
        for (int i = 0; i < speedUpMultiplier; i++)
        {
            if (positionHistory.Count > 0)
            {
                transform.position = positionHistory.Pop();
            }
            else
            {
                StopTimeSpeedUp();
                break;
            }
        }
    }
    private void Rewind()
    {
        if (positionHistory.Count > 0)
        {
            transform.position = positionHistory.Pop();
        }
        else
        {
            StopRewind();
        }
    }

    private void RecordPosition()
    {
        if (positionHistory.Count == 0 || Time.time - positionHistory.Peek().w >= recordTimeStep)
        {
            positionHistory.Push(new Vector4(transform.position.x, transform.position.y, transform.position.z, Time.time));
        }
    }
}
