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

public class network : MonoBehaviour {

    public virtual bool Send(int contID, byte[] byteData)
    {
        return false;
    }
    public virtual bool IsClient()
    {
        return false;
    }
}
