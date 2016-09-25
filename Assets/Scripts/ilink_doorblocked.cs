using UnityEngine;
using System.Collections;

public class ilink_doorblocked : ilink_door {

    public GameObject Airlock = null;
    public override bool Accept(bool on)
    {
        if (!on)
            return true;
        else
        if (Airlock)
        {
            ilink_door i = Airlock.GetComponent<ilink_door>();
            if (i)
            {
                if (!i.IsOpen())
                    return true;
            }
            else
                Debug.Log("No ilock_door found");
        }
        else
            Debug.Log("No Airlock found");
        return false;
    }
}
