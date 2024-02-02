using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpdown : MonoBehaviour
{
    public float SecPerMove;

    private Coroutine routine;

    private void OnEnable()
    {
        routine = StartCoroutine(StartMove());
    }

    private void OnDisable()
    {
        StopCoroutine(routine);
    }

    private IEnumerator StartMove()
    {
        bool isOriginPos = true;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            var moveValue = isOriginPos ? 1 : -1;
            var pos = transform.localPosition;
            pos.y += moveValue;
            transform.localPosition = pos;
            isOriginPos = !isOriginPos;
        }
    }
}
