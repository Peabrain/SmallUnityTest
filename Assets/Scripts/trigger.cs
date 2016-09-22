using UnityEngine;
using System.Collections;

public class trigger : MonoBehaviour {

    public bool auto = false;
    bool changed = false;
    bool isOn = false;
    int InTriggerCounter = 0;
    bool mouseover = false;
	// Use this for initialization
	void OnTriggerEnter(Collider other)
    {
        if(auto) SetActive(true);
    }
    void OnTriggerStay(Collider other)
    {
//        Debug.Log("innen");
    }
    void OnTriggerExit(Collider other)
    {
        if (auto) SetActive(false);
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
    void OnMouseOver()
    {
        mouseover = true;
    }
    void OnMouseExit()
    {
        mouseover = false;
    }
    public bool MouseOver()
    {
        return mouseover;
    }
}
