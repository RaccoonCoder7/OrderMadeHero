using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float degreePerSec;

    private Coroutine routine;


    private void OnEnable()
    {
        routine = StartCoroutine(StartRotate());
    }

    private void OnDisable()
    {
        transform.eulerAngles = Vector3.zero;
        StopCoroutine(routine);
    }

    private IEnumerator StartRotate()
    {
        while (true)
        {
            float degree = degreePerSec * Time.deltaTime;
            Vector3 euler = transform.eulerAngles;
            euler.z += degree;
            if (euler.z > 365)
            {
                euler.z -= 365;
            }
            transform.eulerAngles = euler;
            yield return null;
        }
    }
}
