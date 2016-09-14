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
    TcpClient myclient = null;
    NetworkStream theStream;
    public bool ending = false;
    DateTime Time = DateTime.Now;
    public int port = 40000;
    // Use this for initialization
    void Start () {
        StartClient("localhost", port);
    }
	
	// Update is called once per frame
	void Update () {
        if (ingameContID != -1)
        {
            TimeSpan duration = DateTime.Now - Time;
            if (duration > TimeSpan.FromSeconds(2))
            {
                network_data.ping m = new network_data.ping();
                m.set(ingameContID);
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
                                    s.set(ingameContID);
                                    byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.enter_ship>(s);
                                    Send(ingameContID, data1);

                                    Debug.Log(ingameContID);
                                }
                                break;
                            default:
                                GetComponent<game>().ProcessMessage(ref data);
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
            myclient = new TcpClient(servername, port);
            theStream = myclient.GetStream();
            if (myclient == null)
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
            if (myclient != null)
            {
                myclient.GetStream().Close();
                myclient.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    void Receive(ref List<byte[]> a)
    {
        if (theStream == null) return;
        while (theStream.DataAvailable)
        {
            try
            {
                byte[] recieve = new Byte[8192];
                int len = theStream.Read(recieve, 0, network_utils.nData.Instance.getSize<network_utils.HEADER>());
                network_utils.HEADER header = network_utils.nData.Instance.DeserializeMsg<network_utils.HEADER>(recieve);
                if (header.signum == network_utils.SIGNUM.BIN)
                {
                    int restlaenge = header.size - network_utils.nData.Instance.getSize<network_utils.HEADER>();
                    if (restlaenge > 0)
                    {
                        len += theStream.Read(recieve, network_utils.nData.Instance.getSize<network_utils.HEADER>(), restlaenge);
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
            theStream.Write(byteData, 0, byteData.Count());
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
}
