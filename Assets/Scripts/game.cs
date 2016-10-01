using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class game : receiver {

    public GameObject network = null;
    Dictionary<int, GameObject> ShipList = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> PlayerList = new Dictionary<int, GameObject>();

    DateTime last_clientupdate = new DateTime();
    channel Channel = null;

    void Awake()
    {
    }
    // Use this for initialization
    void Start () {
    }

    public void Init()
    {
        Channel = this.GetComponent<channel>();
        if (!Channel)
            Debug.Log("No Channel");
        else
        {
            if (!Channel.GetNetwork())
                Debug.Log("No Network for Channel");
        }
    }

    // Update is called once per frame
    void Update()
    {
        DateTime now = new DateTime();
        now = DateTime.Now;
        TimeSpan duration = now - last_clientupdate;
        if (duration > TimeSpan.FromMilliseconds(100))
        {
            last_clientupdate = now;
            network_data.move_player m = new network_data.move_player();
            IDictionaryEnumerator i = Channel.FirstEntity();
            while (i.MoveNext())
            {
                GameObject g = (GameObject)i.Value;
                m.set((int)i.Key, Channel.GetChannel());
                m.position = ((GameObject)i.Value).transform.localPosition;
                m.rotation = ((GameObject)i.Value).transform.localRotation;
                byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.move_player>(m);
                Channel.SendToChannel(ref data1);
            }
        }
    }
    public override void ProcessMessage (ref byte [] message)
    {
        network_utils.HEADER header = network_utils.nData.Instance.DeserializeMsg<network_utils.HEADER>(message);
        if (header.signum != network_utils.SIGNUM.BIN)
            return;
        switch (header.command)
        {
        case (int)network_data.COMMANDS.cmove_player:
            {
                network_data.move_player com = network_utils.nData.Instance.DeserializeMsg<network_data.move_player>(message);
                if (Channel.GetNetwork().IsClient())
                {
                    if (Channel.GetNetwork().GetComponent<client>().ingameContID != com.header.containerID)
                    {
                        GameObject g = Channel.GetEntity(com.header.containerID);
                        if (g != null)
                        {
                            g.GetComponent<puppet>().SetTransform(com.position, com.rotation);
                        }
                    }
                }
                else
                {
                    GameObject g = Channel.GetEntity(com.header.containerID);
                    if (g != null)
                    {
                        g.GetComponent<puppet>().SetTransform(com.position, com.rotation);
                    }
                }
            }
            break;
        }
    }
    public void LoadShip(string prefab_name,string object_name)
    {
        GameObject Ship = Instantiate(Resources.Load(prefab_name, typeof(GameObject))) as GameObject;
        channel s = Ship.GetComponent<channel>();
        Ship.GetComponent<puppet>().InitTransform(Ship.transform.localPosition, Ship.transform.localRotation);// rotation);
        network n = GameObject.FindObjectOfType<network>();
        int t = n.AddGameObjectToChannel(Ship);
        s.SetNetwork(n);
        s.SetChannel(t);
        Ship.GetComponent<ship>().Init();
        object_name += "_" + t;
        Ship.name = object_name;
        ShipList[t] = Ship;
        Channel.RegisterEntity(Ship, 0);
    }
    public void AddPlayer(int ID)
    {

    }
    void PlayerControl()
    {

    }
}
