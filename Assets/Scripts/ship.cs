using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ship : channel {

    public List<GameObject> Spawnpoints = new List<GameObject>();
    Dictionary<int, GameObject> Players = new Dictionary<int, GameObject>();
    int spos = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public int SpawnPlayer(int contID,bool IsClient,bool ownPlayer,int spawnpoint)
    {
        if(spawnpoint == -1)
            spawnpoint = spos;
        GameObject Player = Instantiate(Resources.Load("Prefabs/Player", typeof(GameObject)), transform) as GameObject;
        Player.transform.position = Spawnpoints[spawnpoint].transform.position;
        Player.transform.rotation = Spawnpoints[spawnpoint].transform.rotation;
        Players[contID] = Player;
        if(IsClient && ownPlayer)
        {
            Transform t = Player.transform.FindChild("Camera");
            t.gameObject.SetActive(true);
        }
        if (Player) Debug.Log("Enter Ship (" + number + ")" + " player " + contID);
        spos = (spos + 1) % 2;

        if(IsClient == false)
        {
            network_data.create_player m = new network_data.create_player();
            m.set(contID, number);
            m.channel = number;
            m.spawnpoint = spawnpoint;
            byte[] data = network_utils.nData.Instance.SerializeMsg<network_data.create_player>(m);
            mynetwork.Send(contID, data);
        }

        return spawnpoint;
    }
    public override void ProcessMessage(ref byte[] message)
    {
        network_utils.HEADER header = network_utils.nData.Instance.DeserializeMsg<network_utils.HEADER>(message);
        if (header.signum != network_utils.SIGNUM.BIN)
            return;
        switch (header.command)
        {
            case (int)network_data.COMMANDS.center_ship:
                {
                    network_data.enter_ship com = network_utils.nData.Instance.DeserializeMsg<network_data.enter_ship>(message);
                    if (mynetwork.IsClient() == false)
                    {
                        int spawnpoint = SpawnPlayer(com.header.containerID, mynetwork.IsClient(), false, -1);
                    }
                }
                break;
            case (int)network_data.COMMANDS.ccreate_player:
                {
                    network_data.create_player com = network_utils.nData.Instance.DeserializeMsg<network_data.create_player>(message);
                    if (mynetwork.IsClient() == true)
                    {
                        SpawnPlayer(com.header.containerID, mynetwork.IsClient(), mynetwork.GetComponent<client>().ingameContID == com.header.containerID, com.spawnpoint);
                    }
                }
                break;
        }
    }
}
