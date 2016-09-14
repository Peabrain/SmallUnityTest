using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class game : MonoBehaviour {

    public GameObject network = null;
    Dictionary<int, GameObject> ShipList = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> PlayerList = new Dictionary<int, GameObject>();
    void Awake()
    {
    }
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void ProcessMessage (ref byte [] message) {
        network_utils.HEADER header = network_utils.nData.Instance.DeserializeMsg<network_utils.HEADER>(message);
        if (header.signum != network_utils.SIGNUM.BIN)
            return;
        switch (header.command)
        {
            case (int)network_data.COMMANDS.center_ship:
                {
                    network_data.enter_ship com = network_utils.nData.Instance.DeserializeMsg<network_data.enter_ship>(message);
                    if(GetComponent<network>().IsClient() == false)
                    {
                        int spawnpoint = ShipList[com.channel].GetComponent<ship>().SpawnPlayer(com.header.containerID,GetComponent<network>().IsClient(),false,-1);
                    }
                }
                break;
            case (int)network_data.COMMANDS.ccreate_player:
                {
                    network_data.create_player com = network_utils.nData.Instance.DeserializeMsg<network_data.create_player>(message);
                    if (GetComponent<network>().IsClient() == true)
                    {
                        ShipList[com.channel].GetComponent<ship>().SpawnPlayer(com.header.containerID, GetComponent<network>().IsClient(),GetComponent<client>().ingameContID == com.header.containerID,com.spawnpoint);
                    }
                }
                break;
        }
    }
    public void LoadShip(string prefab_name,string object_name)
    {
        int channel = 0;// network.GetComponent<network>().GetFreeChannel();
        object_name += "_" + channel;
        GameObject Ship = Instantiate(Resources.Load(prefab_name, typeof(GameObject))) as GameObject;
        Ship.name = object_name;
        ship s = Ship.GetComponent<ship>();
        s.SetNetwork(GetComponent<network>());
        Ship.GetComponent<ship>().SetParameter(channel);
        ShipList[channel] = Ship;
    }
    public void AddPlayer(int ID)
    {

    }
}
