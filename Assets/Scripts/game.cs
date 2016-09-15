using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class game : channel {

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
    public override void ProcessMessage (ref byte [] message) {
        network_utils.HEADER header = network_utils.nData.Instance.DeserializeMsg<network_utils.HEADER>(message);
        if (header.signum != network_utils.SIGNUM.BIN)
            return;
        switch (header.command)
        {
        }
    }
    public void LoadShip(string prefab_name,string object_name)
    {
        GameObject Ship = Instantiate(Resources.Load(prefab_name, typeof(GameObject))) as GameObject;
        channel s = Ship.GetComponent<channel>();
        network n = GetComponent<network>();
        int channel = n.AddGameObjectToChannel(Ship);
        s.SetNetwork(n);
        s.SetChannel(channel);
        object_name += "_" + channel;
        Ship.name = object_name;
        ShipList[channel] = Ship;
    }
    public void AddPlayer(int ID)
    {

    }
    void PlayerControl()
    {

    }
}
