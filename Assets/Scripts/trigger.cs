using UnityEngine;
using System.Collections;

public class trigger : MonoBehaviour {

    bool changed = false;
    bool isOn = false;
    int InTriggerCounter = 0;
	// Use this for initialization
	void OnTriggerEnter(Collider other)
    {
        SetActive(true);
    }
    void OnTriggerStay(Collider other)
    {
//        Debug.Log("innen");
    }
    void OnTriggerExit(Collider other)
    {
        SetActive(false);
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
