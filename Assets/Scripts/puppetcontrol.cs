using UnityEngine;
using System.Collections;

public class FirstPersonController : puppet {

    // Update is called once per frame
    void Update()
    {
        SetMovementSpeed(Control());
        if (characterController != null)
        {
            characterController.Move(speed * Time.deltaTime);
        }
    }
    public override Vector3 Control()
    {
        // Rotation

        float mouseLeftRight = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime * timescale;
        transform.Rotate(0, mouseLeftRight, 0);

        rotX -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime * timescale;
        if (myCamera != null)
        {
            rotX = Mathf.Clamp(rotX, -updownLimit, updownLimit);
            myCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        }

        // Movement

        float forwardSpeed = Input.GetAxis("Vertical");
        float slideSpeed = Input.GetAxis("Horizontal");

        velY += 9.81f * Time.deltaTime;

        if (characterController.isGrounded)
        {
            velY = Time.deltaTime;
            if (Input.GetButtonDown("Jump"))
            {
                //                velY = -jumpSpeed;
            }
        }
        Vector3 speed = new Vector3(slideSpeed, -velY, forwardSpeed);
        speed = transform.rotation * (speed * movementSpeed);
        return speed;
    }
}
