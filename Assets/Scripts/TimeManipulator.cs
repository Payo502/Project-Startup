using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TimeManipulator : MonoBehaviour
{
    [SerializeField] private InputActionProperty leftTriggerAction;
    [SerializeField] private InputActionProperty leftGripAction;
    [SerializeField] private InputActionProperty xButton;
    [SerializeField] private InputActionProperty yButton;


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

    public void SetLastPickedUpObject(TimeControllableObject timeControllableObject)
    {
        this.lastPickedUpObject = timeControllableObject;
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

        if (lastPickedUpObject != null)
        {
            lastPickedUpObject.ResumeTime();
        }
    }

    private void UpdateTimeControl()
    {
        if (lastPickedUpObject == null)
            return;

        float timeControlFactor = 0f; 

        if (xButton.action.IsPressed())
        {
            // rewind time
            timeControlFactor = -1.0f;
        }
        else if (yButton.action.triggered)
        {
            // speed up time
            timeControlFactor = 1.0f;
        }
        // defaulting to no input

        lastPickedUpObject.ManipulateTime(timeControlFactor);
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
                lastPickedUpObject.OnReleased();
                lastPickedUpObject.ResumeTime();
            }

        }
    }
}
