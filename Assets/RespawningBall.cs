using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningBall : MonoBehaviour
{
    [SerializeField] private float threshold;
    [SerializeField] private float xPos;
    [SerializeField] private float yPos;
    [SerializeField] private float zPos;


    private void FixedUpdate()
    {
        if (transform.position.y < threshold)
        {
            transform.position = new Vector3(xPos, yPos, zPos);
        }
    }
}
