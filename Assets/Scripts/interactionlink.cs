using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Steuerung
// F - aktivieren/deaktivieren

public class interactionlink : MonoBehaviour {

    public GameObject FixPosition = null;
    internal trigger Trigger = null;
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update ()
    {
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
    public void SetTrigger(trigger t)
    {
        Trigger = t;
    }
    public trigger GetTrigger()
    {
        return Trigger;
    }
}
