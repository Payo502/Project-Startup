using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    public delegate void PressurePadStateChange(bool isActivated);
    public event PressurePadStateChange OnStateChange;

    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated)
        {
            isActivated = true;
            OnStateChange?.Invoke(isActivated);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isActivated)
        {
            isActivated = false;
            OnStateChange?.Invoke(isActivated);
        }
    }
}
