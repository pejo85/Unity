using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float scrollSpeed;
    float scrollInput;

    private void Start()
    {
        moveSpeed = 10f;
        scrollSpeed = 150f;
    }

    private void Update()
    {
        scrollInput = Input.GetAxis("Mouse ScrollWheel");
        Check_user_input();
    }

    private void Check_user_input()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-moveSpeed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0,0,-moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, moveSpeed * Time.deltaTime);
        }

        if (scrollInput != 0)
        {
            transform.position += new Vector3(0, -scrollInput * scrollSpeed * Time.deltaTime, 0);
        }
    }

}
