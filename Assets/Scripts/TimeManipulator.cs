using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TimeManipulator : MonoBehaviour
{
    public InputActionProperty leftTriggerAction;
    public InputActionProperty leftGripAction;

    private bool isTimeControlActive = false;

    public TimeControllableObject lastPickedUpObject;

    [Range(0,1)]
    public float intensity;
    public float duration;

    private void Awake()
    {
        //XRBaseInteractor interactable = GetComponent<XRBaseInteractor>();
        //interactable.activated.AddListener(TriggerHapticFeedback);
    }

    private void Update()
    {
        HandleTimeControlActivation();
        if (isTimeControlActive)
        {
            UpdateTimeControl();
        }
    }

    private void HandleTimeControlActivation()
    {
        float leftTriggerValue = leftTriggerAction.action.ReadValue<float>();
        float leftGripValue = leftGripAction.action.ReadValue<float>();

        bool isLeftFistClosed = leftTriggerValue > 0.5f && leftGripValue > 0.5f;

        if (isLeftFistClosed && !isTimeControlActive)
        {
            ActivateTimeControl();
        }
        else if (!isLeftFistClosed && isTimeControlActive)
        {
            DeactivateTimeControl();
        }
    }

    private void ActivateTimeControl()
    {
        isTimeControlActive = true;
    }

    private void DeactivateTimeControl()
    {
        isTimeControlActive = false;

        // If there is an object that was being time-manipulated, resume its time.
        if (lastPickedUpObject != null)
        {
            lastPickedUpObject.ResumeTime();
        }
    }

    private void UpdateTimeControl()
    {
        if (lastPickedUpObject == null)
        {
            Debug.Log("No object picked up");
            return;
        }else
        {
            Debug.Log("Object picked up");
        }

        UnityEngine.XR.InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (!leftHandDevice.isValid)
        {
            return;
        }

        // Check if both grip and trigger are pressed to stop time for the object
        if (isTimeControlActive)
        {
            lastPickedUpObject.StopTime();
        }
        else // If the fist is not closed, then we can check for rotation to rewind or speed up time
        {
            if (leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out Quaternion rotation))
            {
                Vector3 rotationEulers = rotation.eulerAngles;
                float adjustedY = rotationEulers.y > 180f ? rotationEulers.y - 360f : rotationEulers.y;

                // Depending on the rotation, either rewind, speed up, or resume time
                if (adjustedY > 0 && adjustedY <= 90f)
                {
                    lastPickedUpObject.SpeedUpTime();
                }
                else if (adjustedY < 0 && adjustedY >= -90f)
                {
                    lastPickedUpObject.StartRewind();
                }
                else
                {
                    lastPickedUpObject.ResumeTime(); // Resume time if the hand is not rotated to either extreme
                }
            }
        }
    }


    private void TriggerHapticFeedback(XRBaseController controller)
    {
        if(intensity > 0)
        {
            controller.SendHapticImpulse(intensity, duration);
        }
    }

    public void TriggerHapticFeedback(BaseInteractionEventArgs eventArgs)
    {
        if (eventArgs.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            TriggerHapticFeedback(controllerInteractor.xrController);
        }
    }

    private void OnEnable()
    {
        var interactables = FindObjectsOfType<XRGrabInteractable>();
        foreach (var interactable in interactables)
        {
            interactable.selectEntered.AddListener(OnObjectGrabbed);
            interactable.selectExited.AddListener(OnObjectReleased);
        }
    }

    private void OnDisable()
    {
        var interactables = FindObjectsOfType<XRGrabInteractable>();
        foreach (var interactable in interactables)
        {
            interactable.selectEntered.RemoveListener(OnObjectGrabbed);
            interactable.selectExited.RemoveListener(OnObjectReleased);
        }
    }

    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        if (args.interactableObject is XRGrabInteractable grabInteractable)
        {
            TimeControllableObject timeControllableObject = grabInteractable.GetComponent<TimeControllableObject>();
            if (timeControllableObject != null && lastPickedUpObject != timeControllableObject)
            {
                lastPickedUpObject = timeControllableObject;
            }
        }
    }


    private void OnObjectReleased(SelectExitEventArgs args)
    {
        if (args.interactableObject is XRGrabInteractable grabInteractable)
        {
            TimeControllableObject timeControllableObject = grabInteractable.GetComponent<TimeControllableObject>();
            if (timeControllableObject != null)
            {
                //lastPickedUpObject = null;
            }
        }
    }
}
