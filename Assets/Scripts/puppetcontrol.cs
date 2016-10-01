using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class puppetcontrol : puppet {

    public float movementSpeed = 4.0f;
    public float rotationSpeed = 5.0f;
    public float updownLimit = 60.0f;
    public float jumpSpeed = 2.0f;
    public float Gravity = 10.0f;
    internal bool grounded = false;

    internal float velY = 0.0f;
    internal float timescale = 50.0f;

    public float MaxVelocityChange = 10.0f;
    public bool CanJump = true;
    public float JumpHeight = 2.0f;

    internal float rotX = 0;
    internal Vector2 Mousevector;

    GameObject myCamera = null;
//    Dictionary<int,GameObject> ObjectToInteractWith = new Dictionary<int, GameObject>();

    GameObject UI = null;
    channel Channel = null;
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
        UI = GameObject.Find("UI");
        if (UI != null)
            Debug.Log("Found UI");
    }
    public void SetChannel(channel c)
    {
        Channel = c;
    }
    void Update()
    {
        if (!isActiv) return;
        SetMoveVector(new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")));

        UpdateCamera();

        if (UI != null)
        {
            Ray ray = myCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            LayerMask _layerMask = 1 << 9;
            float dist = 1.5f;
            bool use_on = false;
            if (Physics.Raycast(ray, out hit, dist, _layerMask))
            {
                trigger interact = hit.collider.gameObject.GetComponent<trigger>();
                if (interact)
                {
                    use_on = true;
                    if (Channel.GetNetwork().IsClient())
                    {
                        client c = (client)Channel.GetNetwork();
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            interact.SendRequest(c.ingameContID);
                        }
                    }
                }
                else
                {
                }
            }
            Transform m = UI.transform.FindChild("Cursor_Use");
            m.gameObject.SetActive(use_on);
        }

        /*        if (UI != null)
                {
                    foreach (KeyValuePair<int,GameObject> obj in ObjectToInteractWith)
                    {
                        trigger interact = obj.Value.GetComponent<ship>().MouseOver();
                        if (interact != null)
                        {
                            Transform m = UI.transform.FindChild("Cursor_Use");
                            m.gameObject.SetActive(true);
                            if (obj.Value.GetComponent<ship>().GetComponent<channel>().GetNetwork().IsClient())
                            {
                                client c = (client)obj.Value.GetComponent<ship>().GetComponent<channel>().GetNetwork();
                                if (Input.GetKeyDown(KeyCode.F))
                                {
                                    interact.SendRequest(c.ingameContID);
                                }
                            }
                        }
                        else
                        {
                            Transform m = UI.transform.FindChild("Cursor_Use");
                            m.gameObject.SetActive(false);
                        }
                    }
        }
        */
    }
    void FixedUpdate()
    {
        if (!isActiv) return;
        // Calculate how fast we should be moving
        if (grounded)
        {
            Vector3 nn = new Vector3(0, 0, 0);
            Vector3 targetVelocity = Movevector;
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= movementSpeed;

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
/*    public void AddObjectToInteract(GameObject obj)
    {
        ObjectToInteractWith[obj.GetComponent<channel>().GetChannel()] = obj;
    }
*/
    public override void SetActiv(bool a)
    {
        base.SetActiv(a);
        if(isActiv == false)
        {
            Mousevector = new Vector2(0, 0);
            Movevector = new Vector3(0,0,0);
        }
    }
    public void UpdateCamera()
    {
        if (myCamera != null)
        {
            Mousevector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            rotX -= Mousevector.y * 100 * Time.deltaTime;
            rotX = Mathf.Clamp(rotX, -updownLimit, updownLimit);
            myCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);

            float mouseLeftRight = Mousevector.x * Time.deltaTime * 200.0f;
            transform.Rotate(0, mouseLeftRight, 0);
        }
    }
    public void CameraReset()
    {
        rotX = 0;
        rotX = Mathf.Clamp(rotX, -updownLimit, updownLimit);
        myCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        float mouseLeftRight = 0;
        transform.Rotate(0, mouseLeftRight, 0);
    }
}
