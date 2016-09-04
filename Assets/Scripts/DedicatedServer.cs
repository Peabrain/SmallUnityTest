using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DedicatedServer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (SystemInfo.graphicsDeviceID == 0)
        {
            SetupServer();
        }
	}
    void SetupServer() {
        GetComponent<NetworkManager>().StartServer();
    }	
}
