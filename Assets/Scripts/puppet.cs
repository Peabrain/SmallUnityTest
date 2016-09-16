using UnityEngine;
using System.Collections;

public class puppet : MonoBehaviour {

    public float movementSpeed = 5.0f;
    public float rotationSpeed = 5.0f;
    public float updownLimit = 60.0f;
    public float jumpSpeed = 2.0f;

    internal float velY = 0.0f;
    public Vector3 speed = new Vector3(0, 0, 0);
    Vector3 old_speed = new Vector3(0, 0, 0);
    public bool speed_change = false;

    internal float rotX = 0;
    internal float timescale = 50.0f;
    internal CharacterController characterController = null;
    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController != null)
        {
            characterController.Move(speed * Time.deltaTime);
        }
    }
    public void SetMovementSpeed(Vector3 s)
    {
        old_speed = speed;
        speed = s;
        if (old_speed == speed)
            speed_change = false;
        else
            speed_change = true;
    }
    public  virtual Vector3 Control()
    {
        Vector3 speed = new Vector3(0,0,0);
        return speed;
    }
}
