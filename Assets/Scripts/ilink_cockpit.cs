using UnityEngine;
using System.Collections;

// Schiffsteuerung:
// X und Y - hoch und runter
// W und S - Schub vorwärts und rückwärts
// Q und E - Rollen
// A und D - Strafen

public class ilink_cockpit : interactionlink
{
    int user = -1;
    GameObject user_obj = null;
    bool on = false;
    public GameObject User_InterfaceObj = null;

    internal float rotX = 0;
    internal Vector2 Mousevector;

    void Update()
    {
        GameObject GUI = GameObject.Find("GUI");
        if (GUI != null)
        {
            int c = GUI.GetComponent<gui>().GetClientContID();
            if (user == c && on == true)
            {
                if (Input.GetKeyDown(KeyCode.F))
                    GetTrigger().SendRequest(user);
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (user_obj != null)
                        user_obj.GetComponent<puppetcontrol>().UpdateCamera();
                }
                else
                {
                    Mousevector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                    rotX -= Mousevector.y * 100 * Time.deltaTime;
                    float mouseLeftRight = Mousevector.x * Time.deltaTime * 200.0f;
                    Quaternion q = User_InterfaceObj.transform.rotation;
                    q = q * Quaternion.Euler(new Vector3(0, mouseLeftRight, 0));
                    User_InterfaceObj.GetComponent<ship>().transform.localRotation = q;
                }

                float updown = 0f;
                if (Input.GetKey(KeyCode.X))
                    updown = -1f;
                else
                if (Input.GetKey(KeyCode.Y))
                    updown = 1f;
                Vector3 Movevector = new Vector3(Input.GetAxis("Horizontal"), updown, Input.GetAxis("Vertical"));
                User_InterfaceObj.GetComponent<ship>().SetMoveVector(Movevector);

                Vector3 targetVelocity = Movevector;
                targetVelocity = User_InterfaceObj.transform.rotation * targetVelocity;
                //                targetVelocity = User_InterfaceObj.transform.rotation * targetVelocity;
                targetVelocity *= 10.0f;// movementSpeed;
                User_InterfaceObj.GetComponent<ship>().SetVelocity(targetVelocity);
            }
        }
    }
    public override bool Accept(bool on, int contID)
    {
        Debug.Log("user (" + contID + ") cockpit " + on);
        if (user == -1)
        {
            if (on == true)
            {
                user = contID;
                this.on = on;

                GameObject o =Trigger.GetChannel().GetEntity(contID);
                if(FixPosition != null)
                {
                    o.transform.position = FixPosition.transform.position;
                    o.transform.rotation = FixPosition.transform.rotation;
                }

                GameObject GUI = GameObject.Find("GUI");
                if (GUI != null)
                {
                    int c = GUI.GetComponent<gui>().GetClientContID();
                    if (c == contID)
                    {
                        user_obj = o;
                        o.GetComponent<puppetcontrol>().CameraReset();
                        GameObject UI = GameObject.Find("UI");
                        Transform m = UI.transform.FindChild("Cursor_Use");
                        m.gameObject.SetActive(false);
                        Debug.Log("user (" + user + ") enter cockpit");
                    }
                }
                return true;
            }
        }
        else
        {
            if(contID == user)
            {
                this.on = on;
                user = -1;
                GameObject GUI = GameObject.Find("GUI");
                if (GUI != null)
                {
                    int c = GUI.GetComponent<gui>().GetClientContID();
                    if (c == contID)
                    {
                        user_obj = null;
                        User_InterfaceObj.GetComponent<puppet>().SetTransform(User_InterfaceObj.transform.localPosition, User_InterfaceObj.transform.localRotation, puppet.trans_flag_position | puppet.trans_flag_rotation);
                        Debug.Log("user (" + user + ") leave cockpit");
                    }
                }
                return true;
            }
        }
        return false;
    }
    public override void Activate(int contID)
    {
        on = true;
        user = contID;
        GameObject GUI = GameObject.Find("GUI");
        if (GUI != null)
        {
            int c = GUI.GetComponent<gui>().GetClientContID();
            if (c == contID)
                GUI.GetComponent<gui>().PushUserInterface(User_InterfaceObj);
        }
    }
    public override void Deactivate(int contID)
    {
        on = false;
        user = -1;
        GameObject GUI = GameObject.Find("GUI");
        if (GUI != null)
        {
            int c = GUI.GetComponent<gui>().GetClientContID();
            if (c == contID)
                GUI.GetComponent<gui>().PopUserInterface();
        }
    }
    public int GetUserID()
    {
        return user;
    }
}