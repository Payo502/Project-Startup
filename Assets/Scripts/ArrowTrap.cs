using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] List<Transform> arrowSpawnPoints;
    [SerializeField] private float fireForce = 1000f;

    public void FireArrows()
    {
        foreach (Transform arrowSpawnPoint in arrowSpawnPoints)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            arrow.GetComponent<Arrow>().Fire(arrowSpawnPoint.forward * fireForce);
        }
    }
}
