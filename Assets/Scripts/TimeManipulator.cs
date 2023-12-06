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
    [SerializeField] private float sphereCastRadius = 0.5f;

    private Camera mainCamera;

    [SerializeField] private List<TimeControllableObject> selectedObjects = new List<TimeControllableObject>();


    private bool isTimeControlActive = false;

    public TimeControllableObject lastPickedUpObject;

    [Range(0, 1)]
    public float intensity;
    public float duration;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void SetLastPickedUpObject(TimeControllableObject timeControllableObject)
    {
        this.lastPickedUpObject = timeControllableObject;
    }

    private void Update()
    {
        HandleTimeControlActivation();
        ContinuousSphereCastForSelection();

        if (isTimeControlActive)
        {
            UpdateTimeControl();
        }
    }

    private void ContinuousSphereCastForSelection()
    {
        if (mainCamera == null) return;

        RaycastHit[] hits = Physics.SphereCastAll(mainCamera.transform.position, sphereCastRadius, mainCamera.transform.forward, Mathf.Infinity);
        HashSet<TimeControllableObject> hitObjects = new HashSet<TimeControllableObject>();

        foreach (RaycastHit hit in hits)
        {
            TimeControllableObject timeControllableObject = hit.collider.GetComponent<TimeControllableObject>();
            if (timeControllableObject != null)
            {
                hitObjects.Add(timeControllableObject);
                if (!selectedObjects.Contains(timeControllableObject))
                {
                    selectedObjects.Add(timeControllableObject);
                }
            }
        }
        selectedObjects.RemoveAll(obj => !hitObjects.Contains(obj) && obj != null);
    }


    private void HandleTimeControlActivation()
    {
        float leftTriggerValue = leftTriggerAction.action.ReadValue<float>();
        float leftGripValue = leftGripAction.action.ReadValue<float>();

        bool isLeftFistClosed = leftTriggerValue > 0.5f && leftGripValue > 0.5f;

        if (isLeftFistClosed && !isTimeControlActive)
        {
            ActivateTimeControl();
            GameAudioManager.PlaySound(GameAudioManager.Sound.TimeStop);
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

        foreach (var obj in selectedObjects)
        {
            if (obj != null)
            {
                obj.ResumeTime();
            }
        }
    }

    private void UpdateTimeControl()
    {
        float timeControlFactor = GetTimeControlFactor();

        foreach (var obj in selectedObjects)
        {
            if (obj != lastPickedUpObject) 
            {
                obj.ManipulateTime(timeControlFactor);
            }
        }
        if (lastPickedUpObject != null)
        {
            lastPickedUpObject.ManipulateTime(timeControlFactor);
     
        }
    }

    private float GetTimeControlFactor()
    {
        if (xButton.action.IsPressed())
        {
            //GameAudioManager.PlaySound(GameAudioManager.Sound.Rewind, transform.position);


            return -1.0f; 
        }
        else if (yButton.action.triggered)
        {
            return 1.0f;
        }
        return 0f;
    }


    private void TriggerHapticFeedback(XRBaseController controller)
    {
        if (intensity > 0)
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
            if (timeControllableObject != null)
            {

                //selectedObjects.Clear();
                timeControllableObject.isBeingInteractedWith = true;

                lastPickedUpObject = timeControllableObject;

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
                timeControllableObject.isBeingInteractedWith = false;
                lastPickedUpObject.OnReleased();
                lastPickedUpObject.ResumeTime();
            }

        }
    }

    private void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            // Set the color of the Gizmo
            Gizmos.color = Color.blue;

            Gizmos.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 10); // Adjust the length as needed


            Gizmos.DrawWireSphere(mainCamera.transform.position, sphereCastRadius);

            Gizmos.DrawWireSphere(mainCamera.transform.position + mainCamera.transform.forward * 10, sphereCastRadius); // Adjust the position as needed
        }
    }
}
