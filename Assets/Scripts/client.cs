using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;

public class client : network {

    public int ingameContID = -1;
    public bool isInit = false;
    public bool ending = false;
    public int port = 40000;
    SocketData socketdata = new SocketData();
    float timedelta = 0.0f;
    // Use this for initialization
    void Start () {
    }


    public void Connect(string s)
    {
        StartClient(s, port);
    }// Update is called once per frame
    void Update () {
        if (ingameContID != -1)
        {
            TimeSpan duration = DateTime.Now - socketdata.time;
            if (duration > TimeSpan.FromSeconds(1))
            {
                network_data.ping m = new network_data.ping();
                m.set(ingameContID,0);
                m.sendtime = Time.time;
                byte[] data = network_utils.nData.Instance.SerializeMsg<network_data.ping>(m);
                Send(ingameContID,data);
            }
        }
        List<byte[]> datas = new List<byte[]>();
        Receive(ref datas);
        if (datas.Count > 0)
        {
            for (int i = 0;i < datas.Count;i++)
            {
                byte[] data = datas[i];
                if (data.Length > 0)
                {
                    network_utils.HEADER header = network_utils.nData.Instance.DeserializeMsg<network_utils.HEADER>(data);
                    if (header.signum == network_utils.SIGNUM.BIN)
                    {
                        switch (header.command)
                        {
                            case (int)network_data.COMMANDS.cping:
                                {
                                    network_data.ping com = network_utils.nData.Instance.DeserializeMsg<network_data.ping>(data);
                                    float delta = Time.time - com.sendtime;
                                    float t = com.relfecttime + delta / 2.0f;
                                    timedelta = Mathf.Abs(ServerTime.time - t);
                                    ServerTime.time = t;
                                }
                                break;
                            case (int)network_data.COMMANDS.cset_ingame_param:
                                {
                                    network_data.set_ingame_param m = network_utils.nData.Instance.DeserializeMsg<network_data.set_ingame_param>(data);
                                    ingameContID = m.header.containerID;
                                    //                                        m.playername = main.playername;
                                    //                                       byte[] d = network_utils.nData.Instance.SerializeMsg<network_data.set_ingame_param>(m);
                                    //                                      connect.Send(d);

                                    network_data.enter_ship s = new network_data.enter_ship();
                                    s.set(ingameContID,1);
                                    s.shipchannel = m.shipchannel;
                                    byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.enter_ship>(s);
                                    Send(ingameContID, data1);

                                    Debug.Log(ingameContID);
                                }
                                break;
                            default:
                                {
                                    if(ChannelObjectList.ContainsKey(header.channelID))
                                    {
                                        channel c = ChannelObjectList[header.channelID].GetComponent<channel>();
                                        c.ProcessMessage(ref data,c.gameObject.GetComponent<receiver>());
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        StopClient();
    }
    void StartClient(string servername, int port)
    {
        try
        {
            socketdata.socket = new TcpClient(servername, port);
            socketdata.stream = socketdata.socket.GetStream();
            if (socketdata.socket == null)
            {
                ending = true;
            }
            else
            {
                isInit = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    void StopClient()
    {
        try
        {
            if (socketdata.socket != null)
            {
                socketdata.stream.Close();
                socketdata.socket.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    void Receive(ref List<byte[]> a)
    {
        if (socketdata.stream == null) return;
        while (socketdata.stream.DataAvailable)
        {
            try
            {
                byte[] recieve = new Byte[8192];
                int len = socketdata.stream.Read(recieve, 0, network_utils.nData.Instance.getSize<network_utils.HEADER>());
                network_utils.HEADER header = network_utils.nData.Instance.DeserializeMsg<network_utils.HEADER>(recieve);
                if (header.signum == network_utils.SIGNUM.BIN)
                {
                    int restlaenge = header.size - network_utils.nData.Instance.getSize<network_utils.HEADER>();
                    if (restlaenge > 0)
                    {
                        len += socketdata.stream.Read(recieve, network_utils.nData.Instance.getSize<network_utils.HEADER>(), restlaenge);
                    }
                }
                else
                    return;
                byte[] b = new byte[len];
                System.Array.Copy(recieve, 0, b, 0, len);
                a.Add(b);
            }
            catch (Exception e)
            {
                return;
            }
        }

    }

    public override bool Send(int contID, byte[] byteData)
    {
        try
        {
            socketdata.stream.Write(byteData, 0, byteData.Count());
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    string GetMD5Hash(string input)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
        bs = x.ComputeHash(bs);
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        foreach (byte b in bs)
        {
            s.Append(b.ToString("x2").ToLower());
        }
        string password = s.ToString();
        return password;
    }
    public override bool IsClient()
    {
        return true;
    }
    public override void RegisterChannelToSocketdata(int contID, int channel)
    {
        socketdata.channels.Add(channel);
    }
    public override void UnregisterChannelToSocketdata(int contID, int channel)
    {
        socketdata.channels.Remove(channel);
    }
    public void OnGUI()
    {
        string s = "Client contID " + ingameContID;
        GUI.Label(new Rect(2, 10, 150, 100), s);
        s = "ServerTime " + ServerTime.time.ToString("0.0000") + ", Delta " + timedelta.ToString("0.0000");
        GUI.Label(new Rect(2, 10 + 15, 300, 100), s);
    }
}
