using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DedicatedServer : MonoBehaviour {

    int status = 0;
	// Use this for initialization
	void Start () {
        if (SystemInfo.graphicsDeviceID == 0)
        {
            SetupServer();
        }
	}
    void Update() {
        NetworkManager nm = GetComponent<NetworkManager>();
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
                GetComponent<NetworkManager>().StopServer();
                Application.Quit();
            }
        }
    }
    void SetupServer() {
        GetComponent<NetworkManager>().StartServer();
        status = 1;
    }	
}
