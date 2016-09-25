using UnityEngine;
using System.Collections;

public class ilink_door : interactionlink {

    internal bool doorOpen = false;
    public override void Activate()
    {
        if (!GetComponent<Animation>().isPlaying)
            GetComponent<Animation>()["open"].normalizedTime = 0;
        GetComponent<Animation>()["open"].speed = 1;
        GetComponent<Animation>().Play("open");
        doorOpen = true;
    }
    public override void Deactivate()
    {
        if (!GetComponent<Animation>().isPlaying)
            GetComponent<Animation>()["open"].normalizedTime = 1;
        GetComponent<Animation>()["open"].speed = -1;
        GetComponent<Animation>().Play("open");
        doorOpen = false;
    }
    public bool IsOpen()
    {
        return doorOpen;
    }
}
