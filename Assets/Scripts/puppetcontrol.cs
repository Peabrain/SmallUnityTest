using UnityEngine;
using System.Collections;

public class FirstPersonController : puppet {

    public float movementSpeed = 4.0f;
    public float rotationSpeed = 5.0f;
    public float updownLimit = 60.0f;
    public float jumpSpeed = 2.0f;
    public float Gravity = 10.0f;

    internal float velY = 0.0f;
    internal float rotX = 0;
    internal float timescale = 50.0f;
    internal bool grounded = false;

    public float MaxVelocityChange = 10.0f;
    public bool CanJump = true;
    public float JumpHeight = 2.0f;
    public GameObject myCamera = null;
    public bool isActiv = false;
    // Update is called once per frame
    void Awake()
    {
        myCamera = transform.FindChild("Camera").gameObject;
        if (myCamera != null)
        {
            myCamera.SetActive(true);
            //           var head = transform.FindChild("Geo/Head");
            //            head.gameObject.SetActive(false);
        }
    }
    void Update()
    {
    }
    void FixedUpdate()
    {
        if (!isActiv) return;
        // Calculate how fast we should be moving
        if (grounded)
        {
            Vector3 nn = new Vector3(0, 0, 0);
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= movementSpeed;

            float mouseLeftRight = Input.GetAxis("Mouse X") * Time.deltaTime * 200.0f;
            transform.Rotate(0, mouseLeftRight, 0);
            if (myCamera != null)
            {
                rotX -= Input.GetAxis("Mouse Y") * 100 * Time.deltaTime;
                rotX = Mathf.Clamp(rotX, -updownLimit, updownLimit);
                myCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
            }

            nn = transform.rotation * targetVelocity;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = GetComponent<Rigidbody>().velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
        }

        // Jump
        /*            if (canJump && Input.GetButton("Jump"))
                    {
                        GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                    }
        */
        // We apply gravity manually for more tuning control
        Vector3 n = new Vector3(0, -Gravity * GetComponent<Rigidbody>().mass, 0);
        n = transform.parent.rotation * n;
        GetComponent<Rigidbody>().AddForce(n);

        grounded = false;
    }

    void OnCollisionStay()
    {
        grounded = true;
    }
    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * JumpHeight * Gravity);
    }
}
