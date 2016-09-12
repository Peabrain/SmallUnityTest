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

public class DedicatedServer : MonoBehaviour
{
    network_game.SERVER server = null;
    network_game.CLIENT client = null;
    private bool isAtStartup = true;
    /*    public class SocketData
        {
            public SocketData()
            {
                del = false;
                time = new DateTime();
                this.setTime();
            }
            public void setTime()
            {
                time = DateTime.Now;
            }
            public NetworkStream stream;
            public TcpClient socket;
            public DateTime time;
            public bool del;
            public string playername;
        };

        private bool mRunning;
        public static string msg = "";
        public int port = 40000;

        public Thread mThread;
        public TcpListener tcp_Listener = null;

        List<SocketData> socketList= null;
        List<int> socketFreeIDList = null;
        List<int> socketUsedIDList = null;

        TcpClient client = null;
        */
    void Start()
    {
    }
    void OnGUI()
    {
        if (isAtStartup)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Press S for server");
            //            GUI.Label(new Rect(2, 30, 150, 100), "Press B for both");
            GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
        }
        else
        {
            if (server != null)
            {
                string s = "Server is running. (" + server.GetConnectedClients() + ")";
                GUI.Label(new Rect(2, 10, 150, 100),s);
            }
            else
            if (client != null)
            {
                GUI.Label(new Rect(2, 50, 150, 100), "ClientID " + client.ingameContID);
                client.Update();
            }
        }
    }

    void Update()
    {
        if (isAtStartup)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                server = new network_game.SERVER();
                server.StartListening(40000, 4);
                isAtStartup = false;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                client = new network_game.CLIENT();
                client.StartClient("localhost", 40000);
                //                StartClient();
                isAtStartup = false;
            }
        }
        else
        {
            if(server != null)
            {
                server.Update();
            }
            else
            if (client != null)
            {
                client.Update();
            }
        }
    }
}
/*
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
 