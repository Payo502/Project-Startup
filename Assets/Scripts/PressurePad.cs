using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    public delegate void PressurePadStateChange(bool isActivated);
    public event PressurePadStateChange OnStateChange;

    [SerializeField]
    private float deactivationDelay = 2f;
    [SerializeField] private float stabilityCheckDuration = 1f;

    private bool isActivated = false;
    private Coroutine deactivationCoroutine;
    private float lastExitTime;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag("Interactable") || other.CompareTag("Player"))
        {
            isActivated = true;
            OnStateChange?.Invoke(isActivated);

            Debug.Log("Pressurepad Activated");

            if (deactivationCoroutine != null)
            {
                StopCoroutine(deactivationCoroutine);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            lastExitTime = Time.time;
            if (deactivationCoroutine == null)
            {
                deactivationCoroutine = StartCoroutine(DeactivateAfterDelay());
            }
        }
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(stabilityCheckDuration);

        if (Time.time >= lastExitTime + stabilityCheckDuration)
        {
            isActivated = false;
            OnStateChange?.Invoke(isActivated);
            deactivationCoroutine = null;
        }
    }
}
