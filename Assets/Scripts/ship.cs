using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class ship : channel {

    public int netID = 0;
    public List<GameObject> Spawnpoints = new List<GameObject>();
    int spos = 0;
    Dictionary<int, trigger> Triggers = new Dictionary<int, trigger>();
    DateTime last_clientupdate = new DateTime();
    GameObject UI = null;
    GameObject myCam = null;
    // Use this for initialization
    public void Init()
    {
        UI = GameObject.Find("UI");
        if (UI != null)
            Debug.Log("Found UI");
        trigger[] trig = transform.GetComponentsInChildren<trigger>();
        foreach (trigger ti in trig)
        {
            ti.Init(this);
            Triggers[ti.netID] = ti;
            if (GetNetwork().IsClient())
            {
                Collider c = ti.GetComponent<Collider>();
                if (ti.auto)
                    c.enabled = false;
            }
            else
            {
                Collider c = ti.GetComponent<Collider>();
                if (!ti.auto)
                    c.enabled = false;
            }
        }
        Debug.Log("Found " + Triggers.Count + " triggers");
    }
    void Start () {
        last_clientupdate = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetNetwork().IsClient())
        {
            int id = GetNetwork().GetComponent<client>().ingameContID;
            GameObject g = GetEntity(id);

            DateTime now = new DateTime();
            now = DateTime.Now;
            TimeSpan duration = now - last_clientupdate;
            if (duration > TimeSpan.FromMilliseconds(200) || (g != null && g.GetComponent<puppet>().speed_change))
            {
                last_clientupdate = now;
                network_data.move_player m = new network_data.move_player();
                m.set(id, GetChannel());
                m.position = g.transform.position;
                m.rotation = g.transform.rotation;
                m.speed = g.GetComponent<puppet>().speed;
                byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.move_player>(m);
                GetNetwork().Send(id, data1);
            }
            trigger interact = MouseOver();
            if (UI != null)
            {
                if (interact != null)
                {
                    Transform m = UI.transform.FindChild("Cursor_Use");
                    m.gameObject.SetActive(true);
                    if(GetNetwork().IsClient())
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            client c = (client)GetNetwork();
                            interact.SendRequest(c.ingameContID);
                        }
                    }
                }
                else
                {
                    Transform m = UI.transform.FindChild("Cursor_Use");
                    m.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            network_data.move_player m = new network_data.move_player();
            IDictionaryEnumerator i = FirstEntity();
            while (i.MoveNext())
            {
                GameObject g = (GameObject)i.Value;
                if (g.GetComponent<puppet>().speed_change)
                {
                    m.set((int)i.Key, GetChannel());
                    m.position = ((GameObject)i.Value).transform.position;
                    m.rotation = ((GameObject)i.Value).transform.rotation;
                    m.speed = g.GetComponent<puppet>().speed;
                    byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.move_player>(m);
                    SendToChannel(ref data1);
                }
            }
        }
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
        RegisterEntity(Player, contID);
        if(IsClient && ownPlayer)
        {
            Transform t = Player.transform.FindChild("Camera");
            myCam = t.gameObject;
            t.gameObject.SetActive(true);
            Player.AddComponent<FirstPersonController>();
        }
        else
        {
            Player.AddComponent<puppet>();
        }
        if (Player) Debug.Log("Enter Ship (" + GetChannel() + ")" + " player " + contID + " pos: " + position +" rot:" + rotation);

        if(IsClient == false)
        {
            network_data.create_player m = new network_data.create_player();
            m.set(contID, GetChannel());
            m.position = position;
            m.rotation = rotation;
            byte[] data = network_utils.nData.Instance.SerializeMsg<network_data.create_player>(m);
            SendToChannel(ref data);

            List<byte[]> datalist = new List<byte[]>();
            IDictionaryEnumerator r = FirstEntity();
            while(r.MoveNext())
            {
                if (contID != (int)r.Key)
                {
                    m.set((int)r.Key, GetChannel());
                    m.position = ((GameObject)r.Value).transform.position;
                    m.rotation = ((GameObject)r.Value).transform.rotation;
                    byte[]data1 = network_utils.nData.Instance.SerializeMsg<network_data.create_player>(m);
                    GetNetwork().Send(contID, data1);
                }
            }
            foreach (KeyValuePair<int, trigger> j in Triggers)
            {
                trigger t = (trigger)j.Value;
                t.SendTriggerTo(contID);
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
                    if (GetNetwork().IsClient() == false)
                    {
                        GameObject g = GetNextSpawnPoint();
                        SpawnPlayer(com.header.containerID, GetNetwork().IsClient(), false, g.transform.position, g.transform.rotation);
                    }
                }
                break;
            case (int)network_data.COMMANDS.ccreate_player:
                {
                    network_data.create_player com = network_utils.nData.Instance.DeserializeMsg<network_data.create_player>(message);
                    if (GetNetwork().IsClient() == true)
                    {
                        SpawnPlayer(com.header.containerID, GetNetwork().IsClient(), GetNetwork().GetComponent<client>().ingameContID == com.header.containerID, com.position, com.rotation);
                    }
                }
                break;
            case (int)network_data.COMMANDS.cmove_player:
                {
                    network_data.move_player com = network_utils.nData.Instance.DeserializeMsg<network_data.move_player>(message);
                    if (GetNetwork().IsClient())
                    {
                        if (GetNetwork().GetComponent<client>().ingameContID != com.header.containerID)
                        {
                            GameObject g = GetEntity(com.header.containerID);
                            if (g != null)
                            {
                                g.transform.position = com.position;
                                g.transform.rotation = com.rotation;
                                g.GetComponent<puppet>().SetMovementSpeed(com.speed);
                            }
                        }
                    }
                    else
                    {
                        GameObject g = GetEntity(com.header.containerID);
                        if (g != null)
                        {
                            g.transform.position = com.position;
                            g.transform.rotation = com.rotation;
                            g.GetComponent<puppet>().SetMovementSpeed(com.speed);
                        }
                    }
                }
                break;
            case (int)network_data.COMMANDS.cdisconnect:
                {
                    network_data.disconnect com = network_utils.nData.Instance.DeserializeMsg<network_data.disconnect>(message);
                    UnregisterEntity(com.header.containerID);
                }
                break;
            case (int)network_data.COMMANDS.ctrigger:
                {
                    network_data.trigger com = network_utils.nData.Instance.DeserializeMsg<network_data.trigger>(message);
                    Debug.Log("Get triggerdata");
                    if(Triggers.ContainsKey(com.netID))
                    {
                        trigger t = Triggers[com.netID];
                        if (GetNetwork().IsClient())
                        {
                            if(com.accept)
                            {
                                t.SetTrigger(com.count, com.on);
                                t.DoActivate();
                            }
                        }
                        else
                        {
                            t.TriggerRequest();
                            t.DoActivate();
                        }
                    }
                }
                break;
        }
    }
    internal override void UnregisterEntity(int contID)
    {
        GameObject g = GetEntity(contID);
        base.UnregisterEntity(contID);
        if (g != null)
        {
            Destroy(g);
        }
        if (GetNetwork().IsClient() == false)
        {
            network_data.disconnect m = new network_data.disconnect();
            m.set(contID, GetChannel());
            byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.disconnect>(m);
            SendToChannel(ref data1);
        }
    }
    trigger MouseOver()
    {
        trigger ret = null;
        if(GetNetwork().IsClient())
        {
            foreach (KeyValuePair<int, trigger> j in Triggers)
            {
                trigger k = j.Value;
                if(k != null)
                {
                    if (k.auto == false)
                    {
                        if (k.MouseOver())
                        {
                            Collider c = k.GetComponent<Collider>();
                            Vector3 v = myCam.transform.rotation * new Vector3(0,0,1);
                            Ray ray = myCam.GetComponent<Camera>().ScreenPointToRay(new Vector3(0, 0, 0));
                            ray.direction = v;
                            ray.origin = myCam.transform.position;
                            RaycastHit hitinfo;
                            if(c.Raycast(ray, out hitinfo, 1.5f))
                                ret = k;
                        }
                    }
                }
            }
        }
        return ret;
    }
}
