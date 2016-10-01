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
                    if (user_obj != null)
                        user_obj.GetComponent<puppetcontrol>().UpdateCamera();
                if (Input.GetKey(KeyCode.X))
                {
                    User_InterfaceObj.transform.position += new Vector3(0,Time.deltaTime * 1.0f,0);
                }
                else
                if (Input.GetKey(KeyCode.Y))
                {
                    User_InterfaceObj.transform.position -= new Vector3(0, Time.deltaTime * 1.0f, 0);
                }
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
                    }
                }
                Debug.Log("user (" + user + ") enter cockpit");
                return true;
            }
        }
        else
        {
            if(contID == user)
            {
                this.on = on;
                Debug.Log("user (" + user + ") leave cockpit");
                user = -1;
                GameObject GUI = GameObject.Find("GUI");
                if (GUI != null)
                {
                    int c = GUI.GetComponent<gui>().GetClientContID();
                    if (c == contID)
                        user_obj = null;
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
}