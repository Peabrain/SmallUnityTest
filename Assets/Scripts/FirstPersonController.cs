using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {

    public float movementSpeed = 5.0f;
    public float rotationSpeed = 5.0f;
    public GameObject myCamera = null;
    public float updownLimit = 60.0f;
    public float jumpSpeed = 2.0f;

    float velY = 0.0f;

    float rotX = 0;
    float timescale = 50.0f;
    CharacterController characterController = null;
    // Use this for initialization
    void Start () {
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update () {

        // Rotation

        float mouseLeftRight = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime * timescale;
        transform.Rotate(0,mouseLeftRight, 0);

        rotX -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime * timescale;
        if(myCamera != null) {
            rotX = Mathf.Clamp(rotX, -updownLimit, updownLimit);
            myCamera.transform.localRotation = Quaternion.Euler(rotX, 0,0);
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
        if(characterController != null) characterController.Move(speed * Time.deltaTime);
	}
}
