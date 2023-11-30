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
    [SerializeField]
    [Range(0, 1)]
    private float shakeIntensity = 0.1f;
    [SerializeField]
    [Range(0, 1)]
    private float maxShakeIntensity = 0.1f;
    [SerializeField]
    private float shakeSpeed = 50f;
    
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
        }
        else
        {
            activatedPads--;
        }

        CheckDoorActivation();
    }

    private void CheckDoorActivation()
    {
        if (activatedPads == pressurePads.Count && !isOpen)
        {
            isOpen = true;
            StartCoroutine(MoveAndShakeDoor(door.transform.position + new Vector3(0, moveDistance, 0)));
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
            StartCoroutine(MoveAndShakeDoor(door.transform.position + new Vector3(0, moveDistance, 0)));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isOpen = false;
    }

    private IEnumerator MoveAndShakeDoor(Vector3 targetPosition)
    {
        Vector3 originalPosition = door.transform.position;
        float totalDistance = Vector3.Distance(originalPosition, targetPosition);

        while (Vector3.Distance(door.transform.position, targetPosition) > 0.01f)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPosition, moveSpeed * Time.deltaTime);
/*
            float currentDistance = Vector3.Distance(door.transform.position, originalPosition);
            float progress = currentDistance / totalDistance;

            float shakeIntensity = maxShakeIntensity * Mathf.Sin(progress * Mathf.PI);

            float shakeAmount = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            door.transform.position += new Vector3(shakeAmount, 0, shakeAmount);*/


            yield return null;
        }
        door.transform.position = targetPosition;
    }
}
