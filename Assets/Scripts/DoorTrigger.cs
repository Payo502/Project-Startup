using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private List<PressurePad> pressurePads;

    [SerializeField]
    private float moveDistance = 2.5f;
    [SerializeField]
    private float moveSpeed = 1f;
    
    private bool isOpen = false;

    private int activatedPads = 0;

    private void Start()
    {
        foreach (PressurePad pressurePad in pressurePads)
        {
            pressurePad.OnStateChange += HandlePressurePadStateChange;
        }
    }

    private void OnDestroy()
    {
        foreach (PressurePad pressurePad in pressurePads)
        {
            pressurePad.OnStateChange -= HandlePressurePadStateChange;
        }
    }

    private void HandlePressurePadStateChange(bool isActivated)
    {
        if (isActivated)
        {
            activatedPads++;
            Debug.Log("Pressure Pad Activated. Total Activated: " + activatedPads);
        }
        else
        {
            activatedPads--;
            Debug.Log("Pressure Pad Deactivated. Total Activated: " + activatedPads);
        }

        CheckDoorActivation();
    }

    private void CheckDoorActivation()
    {
        Debug.Log("Checking Door Activation. Required: " + pressurePads.Count);
        if (activatedPads == pressurePads.Count && !isOpen)
        {
            isOpen = true;
            Debug.Log("Opening Door.");
            StartCoroutine(MoveDoor(door.transform.position + new Vector3(0, moveDistance, 0)));
            GameAudioManager.PlaySound(GameAudioManager.Sound.DoorSlide, transform.position);
        }
        else if (activatedPads < pressurePads.Count && isOpen)
        {
            //isOpen = false;
            // if needed to open door
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOpen)
        {
            isOpen = true;
            StartCoroutine(MoveDoor(door.transform.position + new Vector3(0, moveDistance, 0)));

            Debug.Log("Playing Door slide sound");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isOpen = false;
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    { 
        while (Vector3.Distance(door.transform.position, targetPosition) > 0.01f)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        door.transform.position = targetPosition;
    }
}
