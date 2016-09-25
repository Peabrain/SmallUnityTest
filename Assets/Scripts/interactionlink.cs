﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class interactionlink : MonoBehaviour {

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update ()
    {
    }

    public virtual bool Accept(bool on)
    {
        return true;
    }
    public virtual void Activate()
    {
        if (!GetComponent<Animation>().isPlaying)
            GetComponent<Animation>()["open"].normalizedTime = 0;
        GetComponent<Animation>()["open"].speed = 1;
        GetComponent<Animation>().Play("open");
    }
    public virtual void Deactivate()
    {
        if (!GetComponent<Animation>().isPlaying)
            GetComponent<Animation>()["open"].normalizedTime = 1;
        GetComponent<Animation>()["open"].speed = -1;
        GetComponent<Animation>().Play("open");
    }
}
