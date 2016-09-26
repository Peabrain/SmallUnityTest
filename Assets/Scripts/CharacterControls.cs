using UnityEngine;
using System.Collections;

public class CharacterControls : MonoBehaviour {

    public float speed = 10.0f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    private bool grounded = false;

//    public float movementSpeed = 5.0f;
//    public float rotationSpeed = 5.0f;
    public float updownLimit = 60.0f;
    internal float rotX = 0;
    //    public float jumpSpeed = 2.0f;

    public GameObject myCamera = null;
    // Update is called once per frame
    void Awake()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().useGravity = false;
        myCamera = transform.FindChild("Camera").gameObject;
        if (myCamera != null)
        {
            myCamera.SetActive(true);
            //           var head = transform.FindChild("Geo/Head");
            //            head.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (grounded)
        {
            // Calculate how fast we should be moving
            Vector3 nn = new Vector3(0, 0, 0);
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

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

            // Jump
/*            if (canJump && Input.GetButton("Jump"))
            {
                GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }
*/
        }

        // We apply gravity manually for more tuning control
        Vector3 n = new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0);
        n = transform.rotation * n;
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
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
