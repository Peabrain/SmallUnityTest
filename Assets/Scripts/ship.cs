using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ship : channel {

    public List<GameObject> Spawnpoints = new List<GameObject>();
    int spos = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    GameObject GetNextSpawnPoint()
    {
        int s = spos;
        spos = (spos + 1) % 2;
        return Spawnpoints[s];
    }
    public void SpawnPlayer(int contID,bool IsClient,bool ownPlayer,Vector3 position,Quaternion rotation)
    {
        GameObject Player = Instantiate(Resources.Load("Prefabs/Player", typeof(GameObject)), transform) as GameObject;
        Player.transform.position = position;
        Player.transform.rotation = rotation;
        entity[contID] = Player;
        if(IsClient && ownPlayer)
        {
            Transform t = Player.transform.FindChild("Camera");
            t.gameObject.SetActive(true);
            FirstPersonController f = Player.GetComponent<FirstPersonController>();
            f.enabled = true;
        }
        if (Player) Debug.Log("Enter Ship (" + number + ")" + " player " + contID + " pos: " + position +" rot:" + rotation);

        if(IsClient == false)
        {
            network_data.create_player m = new network_data.create_player();
            m.set(contID, number);
            m.channel = number;
            m.position = position;
            m.rotation = rotation;
            byte[] data = network_utils.nData.Instance.SerializeMsg<network_data.create_player>(m);
            SendToChannel(ref data);
            foreach (KeyValuePair<int, GameObject> i in entity)
            {
                if(contID != i.Key)
                {
                    m.set(i.Key, number);
                    m.position = i.Value.transform.position;
                    m.rotation = i.Value.transform.rotation;
                    byte[]data1 = network_utils.nData.Instance.SerializeMsg<network_data.create_player>(m);
                    mynetwork.Send(contID, data1);
                }
            }
        }
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
                        GameObject g = GetNextSpawnPoint();
                        SpawnPlayer(com.header.containerID, mynetwork.IsClient(), false, g.transform.position,g.transform.rotation);
                    }
                }
                break;
            case (int)network_data.COMMANDS.ccreate_player:
                {
                    network_data.create_player com = network_utils.nData.Instance.DeserializeMsg<network_data.create_player>(message);
                    if (mynetwork.IsClient() == true)
                    {
                        SpawnPlayer(com.header.containerID, mynetwork.IsClient(), mynetwork.GetComponent<client>().ingameContID == com.header.containerID, com.position,com.rotation);
                    }
                }
                break;
        }
    }
}
