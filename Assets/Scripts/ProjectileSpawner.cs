using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject notch;

    private XRGrabInteractable bow;
    private bool isProjectileNotched = false;
    private GameObject currentProjectile = null;

    private void Start()
    {
        bow = GetComponent<XRGrabInteractable>();
        PullInteraction.PullActionReleased += NotchEmpty;
    }

    private void OnDestroy()
    {
        PullInteraction.PullActionReleased -= NotchEmpty;
    }

    private void Update()
    {
        if (bow.isSelected && isProjectileNotched == false)
        {
            isProjectileNotched = true;
            StartCoroutine("DelayedSpawn");
        }
        if(!bow.isSelected && currentProjectile != null)
        {
            Destroy(currentProjectile);
            NotchEmpty(1f);
        }
    }

    private void NotchEmpty(float value)
    {
        isProjectileNotched = false;
        currentProjectile = null;
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.1f);
        currentProjectile = Instantiate(projectile, notch.transform);
    }
}
