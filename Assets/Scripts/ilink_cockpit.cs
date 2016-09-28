using UnityEngine;
using System.Collections;

public class ilink_cockpit : interactionlink
{
    int user = -1;
    bool on = false;
    public override bool Accept(bool on, int contID)
    {
        Debug.Log("user (" + contID + ") cockpit " + on);
        if (user == -1)
        {
            if (on == true)
            {
                user = contID;
                this.on = on;
                Debug.Log("user (" + user + ") enter cockpit");
                return true;
            }
        }
        else
        {
            if(contID == user)
            {
                if (on == false)
                {
                    this.on = on;
                    Debug.Log("user (" + user + ") leave cockpit");
                    user = -1;
                    return true;
                }
            }
        }
        return false;
    }
    public override void Activate(int contID)
    {
        on = true;
        user = contID;
    }
    public override void Deactivate(int contID)
    {
        on = false;
        user = -1;
    }
}