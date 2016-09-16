﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class ship : channel {

    public List<GameObject> Spawnpoints = new List<GameObject>();
    int spos = 0;
    DateTime last_clientupdate = new DateTime();
    // Use this for initialization
    void Start () {
        last_clientupdate = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if (mynetwork.IsClient() == false)
        {
/*            network_data.move_player m = new network_data.move_player();
            foreach (KeyValuePair<int, GameObject> i in entity)
            {
                m.set(i.Key, number);
                m.position = i.Value.transform.position;
                m.rotation = i.Value.transform.rotation;
                byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.move_player>(m);
                SendToChannel(ref data1);
            }
  */      }
        else
        {
            int id = mynetwork.GetComponent<client>().ingameContID;
            GameObject g = GetEntity(id);

            DateTime now = new DateTime();
            now = DateTime.Now;
            TimeSpan duration = now - last_clientupdate;
            if (duration > TimeSpan.FromMilliseconds(200) || g.GetComponent<puppet>().speed_change)
            {
                last_clientupdate = now;
                network_data.move_player m = new network_data.move_player();
                m.set(id, number);
                m.position = g.transform.position;
                m.rotation = g.transform.rotation;
                m.speed = g.GetComponent<puppet>().speed;
                byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.move_player>(m);
                mynetwork.Send(id, data1);
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
            t.gameObject.SetActive(true);
            Player.AddComponent<FirstPersonController>();
        }
        else
        {
            Player.AddComponent<puppet>();
        }
        if (Player) Debug.Log("Enter Ship (" + number + ")" + " player " + contID + " pos: " + position +" rot:" + rotation);

        if(IsClient == false)
        {
            network_data.create_player m = new network_data.create_player();
            m.set(contID, number);
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
                    m.set((int)r.Key, number);
                    m.position = ((GameObject)r.Value).transform.position;
                    m.rotation = ((GameObject)r.Value).transform.rotation;
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
                        SpawnPlayer(com.header.containerID, mynetwork.IsClient(), false, g.transform.position, g.transform.rotation);
                    }
                }
                break;
            case (int)network_data.COMMANDS.ccreate_player:
                {
                    network_data.create_player com = network_utils.nData.Instance.DeserializeMsg<network_data.create_player>(message);
                    if (mynetwork.IsClient() == true)
                    {
                        SpawnPlayer(com.header.containerID, mynetwork.IsClient(), mynetwork.GetComponent<client>().ingameContID == com.header.containerID, com.position, com.rotation);
                    }
                }
                break;
            case (int)network_data.COMMANDS.cmove_player:
                {
                    network_data.move_player com = network_utils.nData.Instance.DeserializeMsg<network_data.move_player>(message);
                    if (mynetwork.IsClient())
                    {
                        if (mynetwork.GetComponent<client>().ingameContID != com.header.containerID)
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
                            network_data.move_player m = new network_data.move_player();
                            IDictionaryEnumerator i = FirstEntity();
                            while (i.MoveNext())
                            {
                                m.set((int)i.Key, number);
                                m.position = ((GameObject)i.Value).transform.position;
                                m.rotation = ((GameObject)i.Value).transform.rotation;
                                m.speed = ((GameObject)i.Value).GetComponent<puppet>().speed;
                                byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.move_player>(m);
                                SendToChannel(ref data1);
                            }
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
        if (mynetwork.IsClient() == false)
        {
            network_data.disconnect m = new network_data.disconnect();
            m.set(contID, number);
            byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.disconnect>(m);
            SendToChannel(ref data1);
        }
    }
}
