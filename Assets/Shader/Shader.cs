using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shader : MonoBehaviour
{

    public UnityEngine.Shader customShader;

    void Start()
    {
        Material material = GetComponent<Renderer>().material;

        material.shader = customShader;
    }
}


