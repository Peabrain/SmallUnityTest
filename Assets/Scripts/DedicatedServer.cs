using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine.Networking;

/*public class DedicatedServer : MonoBehaviour
{
    private bool mRunning;
    public static string msg = "";

    public Thread mThread;
    public TcpListener tcp_Listener = null;

    void Awake()
    {
        mRunning = true;
        ThreadStart ts = new ThreadStart(Receive);
        mThread = new Thread(ts);
        mThread.Start();
        print("Thread done...");
    }

    public void stopListening()
    {
        mRunning = false;
    }

    void Receive()
    {
        tcp_Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8001);
        tcp_Listener.Start();
        print("Server Start");
        while (mRunning)
        {
            // check if new connections are pending, if not, be nice and sleep 100ms
            if (!tcp_Listener.Pending())
            {
                Thread.Sleep(100);
            }
            else
            {
                Socket ss = tcp_Listener.AcceptSocket();
//                BonePos rec = new BonePos();
//                byte[] tempbuffer = new byte[10000];
//                ss.Receive(tempbuffer); // received byte array from client
//                rec.AssignFromByteArray(tempbuffer); // my own datatype
            }
        }
    }

    void Update()
    {

    }

    void OnApplicationQuit()
    { // stop listening thread
        stopListening();// wait for listening thread to terminate (max. 500ms)
        mThread.Join(500);
    }
}
/**/

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
