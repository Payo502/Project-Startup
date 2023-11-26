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
        }

        UnityEngine.XR.InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (!leftHandDevice.isValid)
        {
            Debug.Log("Left hand device is not valid");
            return;
        }

        if (leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 joystickValue))
        {
            // Use the x-axis of the joystick for time manipulation
            float joystickX = joystickValue.x;

            // Clamp the value to ensure it's within the range of -1 to 1
            joystickX = Mathf.Clamp(joystickX, -1f, 1f);


            lastPickedUpObject.ManipulateTime(joystickX);
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
                // Reset the time manipulation state when the object is picked up
                lastPickedUpObject.ResetTimeManipulation();
                lastPickedUpObject.OnPickedUp();
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
                // Handle any necessary logic when the object is released
                lastPickedUpObject.OnReleased();
                lastPickedUpObject.ResumeTime();
            }

        }
    }
}
