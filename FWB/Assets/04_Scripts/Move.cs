using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed;

    void Start()
    {
        Debug.Log("Hello World!");
    }

    void Update()
    {
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDir += Vector3.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir += Vector3.back;
        }

        if (moveDir != Vector3.zero)
        {
            transform.Translate(moveDir * Time.deltaTime * moveSpeed);
        }
    }
}
