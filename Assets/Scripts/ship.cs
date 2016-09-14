using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ship : MonoBehaviour {

    public network mynetwork = null;
    int channel = 0;
    public List<GameObject> Spawnpoints = new List<GameObject>();
    Dictionary<int, GameObject> Players = new Dictionary<int, GameObject>();
    int spos = 0;
	// Use this for initialization
    public void SetParameter(int channel)
    {
        this.channel = channel;
    }
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
        if (Player) Debug.Log("Enter Ship (" + channel + ")" + " player " + contID);
        spos = (spos + 1) % 2;

        if(IsClient == false)
        {
            network_data.create_player m = new network_data.create_player();
            m.set(contID);
            m.channel = channel;
            m.spawnpoint = spawnpoint;
            byte[] data = network_utils.nData.Instance.SerializeMsg<network_data.create_player>(m);
            mynetwork.Send(contID, data);
        }

        return spawnpoint;
    }
    public void SetNetwork(network n)
    {
        mynetwork = n;
    }
}
