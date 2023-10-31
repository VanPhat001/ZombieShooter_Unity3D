using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject fpsCamera;
    public GameObject gunWrapper;
    public GameObject grenadeWrapper;

    public float speed = 5f;
    public float speedUpRate = 1.8f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Move()
    {
        float speed = Time.deltaTime * this.speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= this.speedUpRate;
        }

        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(Vector3.forward * speed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(Vector3.back * speed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(Vector3.left * speed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(Vector3.right * speed);
        }
    }

    void RotateBody()
    {
        float horizontal = Input.GetAxis("Mouse X");
        if (horizontal != 0)
        {
            this.transform.Rotate(0, horizontal, 0);
        }
    }

    void RotateHead()
    {
        float vertical = Input.GetAxis("Mouse Y");
        if (vertical != 0)
        {
            this.fpsCamera.transform.Rotate(-vertical * 0.7f, 0, 0);

            Vector3 euler = this.fpsCamera.transform.rotation.eulerAngles;
            float maxBottomRotation = 30;
            float maxTopRotation = -30;
            float newX = 0;

            if (maxBottomRotation < euler.x && euler.x < 360 + maxTopRotation)
            {
                float dBot = euler.x - maxBottomRotation;
                float dTop = 360 + maxTopRotation - euler.x;
                newX = dBot < dTop ? maxBottomRotation : 360 + maxTopRotation;
            }

            if (newX != 0)
            {
                this.fpsCamera.transform.rotation = Quaternion.Euler(newX, euler.y, euler.z);
            }
        }
    }

    private void Update()
    {
        Move();
        RotateBody();
        RotateHead();
    }
}
