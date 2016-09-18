using UnityEngine;
using System.Collections;

public class trigger : MonoBehaviour {

    public int netID = 0;
    public channel mychannel = null;
    public GameObject door = null;

    bool changed = false;
    bool isOn = false;
    int InTriggerCounter = 0;
	// Use this for initialization
	void OnTriggerEnter(Collider other)
    {
        if (door != null)
        {
            SetActive(true);
        }
    }
    void OnTriggerStay(Collider other)
    {
//        Debug.Log("innen");
    }
    void OnTriggerExit(Collider other)
    {
        if (door != null)
        {
            SetActive(false);
        }
    }
    public void Activate()
    {
        if (!door.GetComponent<Animation>().isPlaying)
            door.GetComponent<Animation>()["open"].normalizedTime = 0;
        door.GetComponent<Animation>()["open"].speed = 1;
        door.GetComponent<Animation>().Play("open");
    }
    public void Deactivate()
    {
        if (!door.GetComponent<Animation>().isPlaying)
            door.GetComponent<Animation>()["open"].normalizedTime = 1;
        door.GetComponent<Animation>()["open"].speed = -1;
        door.GetComponent<Animation>().Play("open");
    }
    public bool IsOn()
    {
        return isOn;
    }
    public void SetActive(bool a)
    {
        isOn = a;
        if(isOn)
        {
            if (InTriggerCounter == 0)
                changed = true;
            InTriggerCounter++;
        }
        else
        {
            InTriggerCounter--;
            if (InTriggerCounter == 0)
                changed = true;
        }
    }
    public void ClearChanged()
    {
        changed = false;
    }
    public bool HasChanged()
    {
        return changed;
    }
}
