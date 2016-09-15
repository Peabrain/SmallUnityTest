using UnityEngine;
using System.Collections;

public class puppet : MonoBehaviour {

    public float movementSpeed = 5.0f;
    public float rotationSpeed = 5.0f;
    public GameObject myCamera = null;
    public float updownLimit = 60.0f;
    public float jumpSpeed = 2.0f;

    internal float velY = 0.0f;
    Vector3 speed = new Vector3(0, 0, 0);
    public bool speed_change = false;

    internal float rotX = 0;
    internal float timescale = 50.0f;
    internal CharacterController characterController = null;
    // Use this for initialization
    void Start()
    {
        if (myCamera != null)
        {
            myCamera.SetActive(true);
            //           var head = transform.FindChild("Geo/Head");
            //            head.gameObject.SetActive(false);
        }
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        SetMovementSpeed(Control());
        if (characterController != null)
        {
            characterController.Move(speed * Time.deltaTime);
        }
    }
    public void SetMovementSpeed(Vector3 s)
    {
        speed = s;
    }
    public  virtual Vector3 Control()
    {
        Vector3 speed = new Vector3(0,0,0);
        return speed;
    }
}
