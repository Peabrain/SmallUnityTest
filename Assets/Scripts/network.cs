using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Text;

public abstract class network : MonoBehaviour {

    void Awake()
    {
        //      DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {
    }

    public abstract bool Send(int contID, byte[] byteData);
}
