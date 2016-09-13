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

public class DedicatedServer : MonoBehaviour {

    GameObject Ship1 = null;
    int status = 0;
	// Use this for initialization
	void Start () {
        Ship1 = Instantiate(Resources.Load("Prefabs/Ship1", typeof(GameObject))) as GameObject;
        if (SystemInfo.graphicsDeviceID == 0)
        {
            SetupServer();
        }
	}
    void Update() {
        NetworkManager nm = Ship1.GetComponent<NetworkManager>();
        if (status == 1) {
            Debug.Log(nm.numPlayers);
            if (nm.numPlayers > 0)
            {
                status = 2;
            }
        }
        else
        if (status == 2)
        {
            if (nm.numPlayers == 0) {
                nm.StopServer();
                Application.Quit();
            }
        }
    }
    void SetupServer() {
        Ship1.GetComponent<NetworkManager>().StartServer();
        status = 1;
    }	
}
/**/
 