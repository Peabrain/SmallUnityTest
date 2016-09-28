using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Steuerung
// F - aktivieren/deaktivieren

public class interactionlink : MonoBehaviour {

    public GameObject FixPosition = null;
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update ()
    {
    }

    public bool CheckInteract()
    {
        return Input.GetKeyDown(KeyCode.F);
    }

    public virtual bool Accept(bool on,int contID)
    {
        return true;
    }
    public virtual void Activate(int contID)
    {
        Activate();
    }
    public virtual void Deactivate(int contID)
    {
         Deactivate();
    }
    public virtual void Activate()
    {
    }
    public virtual void Deactivate()
    {
    }
}
