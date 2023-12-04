using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public float threshold;

    private void FixedUpdate()
    {
        if (transform.position.y  < threshold)
        {
            transform.position = new Vector3(7.74f, 0.08258295f, 2.78f);
        }
    }
}
