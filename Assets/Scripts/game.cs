using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class game : MonoBehaviour {

    public GameObject network = null;
    Dictionary<int, GameObject> ShipList = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> PlayerList = new Dictionary<int, GameObject>();
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//        if(network != null) network.GetComponent<network>().Update();
    }
/*    void LoadShip(string prefab_name,string object_name)
    {
        int channel = network.GetComponent<network>().GetFreeChannel();
        object_name += "_" + channel;
        GameObject Ship = Instantiate(Resources.Load(prefab_name, typeof(GameObject))) as GameObject;
        Ship.name = object_name;
        Ship.GetComponent<ship>().SetParameter(channel);
        ShipList[channel] = Ship;
    }
    public void StartServer()
    {
        if (network != null)
        {
            network.GetComponent<network>().StartServer();
            LoadShip("Prefabs/Ship1", "Ship");
        }
    }
    public void StartClient()
    {
        if (network != null)
        {
            network.GetComponent<network>().StartClient();
            LoadShip("Prefabs/Ship1", "Ship");
        }
    }
    public void AddPlayer(int ID)
    {

    }
*/
}
