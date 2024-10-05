using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseEffect : MonoBehaviour
{
    public Material noiseMaterial;
    public float updateInterval = 1.0f;
    private float customTime = 0.0f;
    private float elapsedTime = 0.0f;

    private void Start()
    {
        noiseMaterial = GetComponent<RawImage>().material;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= updateInterval)
        {
            customTime += updateInterval;
            noiseMaterial.SetFloat("_CustomTime", customTime);
            
            elapsedTime = 0.0f;
        }
    }
}
