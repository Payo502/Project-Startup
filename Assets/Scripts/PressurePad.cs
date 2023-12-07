using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    public delegate void PressurePadStateChange(bool isActivated);
    public event PressurePadStateChange OnStateChange;

    [SerializeField] private float deactivationDelay = 1f;

    [SerializeField] private Material activatedMaterial;
    [SerializeField] private Material deactivatedMaterial;
    [SerializeField] private MeshRenderer parentRender;


    public int objectsOnPad = 0;
    public bool isActivated = false;
    private Coroutine deactivationCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (IsRelevantObject(other))
        {
            objectsOnPad++;
            ActivatePressurePad();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsRelevantObject(other))
        {
            objectsOnPad--;
            if (objectsOnPad <= 0 && deactivationCoroutine == null)
            {
                deactivationCoroutine = StartCoroutine(DeactivateAfterDelay());
            }
        }
    }

    private void ActivatePressurePad()
    {
        if (!parentRender.material.Equals(activatedMaterial))
        {
            parentRender.material = activatedMaterial;
            OnStateChange?.Invoke(true);
        }

        if (deactivationCoroutine != null)
        {
            StopCoroutine(deactivationCoroutine);
            deactivationCoroutine = null;
        }
    }

    private void DeactivatePressurePad()
    {
        parentRender.material = deactivatedMaterial;
        OnStateChange?.Invoke(false);
        deactivationCoroutine = null;
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(deactivationDelay);
        if (objectsOnPad <= 0)
        {
            DeactivatePressurePad();
        }
    }

    private bool IsRelevantObject(Collider other)
    {
        return other.CompareTag("Interactable") || other.CompareTag("Player");
    }
}
