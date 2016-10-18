using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
//using StateObjectns;
using System.Runtime.InteropServices;
//using network_lobby;
using network_data;
using network_utils;

public class server : network {

    public int port = 40000;
    public DateTime time;
    List<SocketData> socketList;
    List<int> socketFreeIDList;
    List<int> socketUsedIDList;
    TcpListener listener;
    public bool endgame = false;
    bool beginning = false;
    int maxplayer;
    // Use this for initialization
    void Awake()
    {
        Debug.Log("Create Server");
        Console.WriteLine("Create Server");
        socketList = new List<SocketData>();
        socketFreeIDList = new List<int>();
        socketUsedIDList = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            socketFreeIDList.Add(i);
            socketList.Add(new SocketData());
        }
        StartListening(port, 4);
    }
    void Start () {

    }

    public override bool IsClient()
    {
        return false;
    }

    public void OnGUI()
    {
        string s = "Server is running. (" + GetConnectedClients() + ")";
        GUI.Label(new Rect(2, 10, 150, 100), s);
        s = "ServerTime " + Time.time.ToString("0.0000");
        GUI.Label(new Rect(2, 10 + 15, 150, 100), s);
    }
    // Update is called once per frame
    void Update () {
        CheckPingOut();
        checkDel();
        List<network_utils.cl_data> messages = new List<network_utils.cl_data>();
        messages.Clear();
        Receive(ref messages);
        foreach (network_utils.cl_data bb in messages)
        {
            int count = 0;
            bb.data.CopyTo(bb.data, count);
            network_utils.HEADER header = network_utils.nData.Instance.DeserializeMsg<network_utils.HEADER>(bb.data);
            if (header.signum != network_utils.SIGNUM.BIN)
                continue;
            SetPing(bb.containerID);
            switch (header.command)
            {
/*                case (int)network_data.COMMANDS.cend_ingame:
                    {
                        network_data.end_ingame com = network_utils.nData.Instance.DeserializeMsg<network_data.end_ingame>(bb.data);
                        socketList[com.header.containerID].del = true;
                    }
                    break;
*/
                case (int)network_data.COMMANDS.cping:
                    {
                        network_data.ping com = network_utils.nData.Instance.DeserializeMsg<network_data.ping>(bb.data);
                        com.relfecttime = Time.time;
                        byte[] data = network_utils.nData.Instance.SerializeMsg<network_data.ping>(com);
                        Send(com.header.containerID, data);
                    }
                    break;
                case (int)network_data.COMMANDS.cset_ingame_param:
                    {
                        network_data.set_ingame_param com = network_utils.nData.Instance.DeserializeMsg<network_data.set_ingame_param>(bb.data);
                        socketList[com.header.containerID].playername = com.playername;
                    }
                    break;
                default:
                    {
                        channel c = ChannelObjectList[header.channelID].GetComponent<channel>();
                        c.ProcessMessage(ref bb.data,c.gameObject.GetComponent<receiver>());
                    }
                    break;
            }
        }
        if (beginning == true && socketUsedIDList.Count == 0)
            endgame = true;
    }
    int GetConnectedClients()
    {
        return socketUsedIDList.Count;
    }
    void StartListening(int port, int maxplayers)
    {
        try
        {
            this.maxplayer = maxplayers;
            listener = new TcpListener(IPAddress.Any, port);

            // Start listening for client requests.
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(Listen), listener);
            Debug.Log("Waiting for a Game connection...");
            Console.WriteLine("Waiting for a Game connection...");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public int GetFreeID()
    {
        int id = socketFreeIDList[0];
        socketFreeIDList.RemoveAt(0);
        return id;
    }
    public void AddFreeID(int id)
    {
        socketUsedIDList.Remove(id);
        socketFreeIDList.Add(id);
    }

    public void Listen(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        TcpClient client = listener.EndAcceptTcpClient(ar);

        if (socketFreeIDList.Count == 0)
        {
            socketFreeIDList.Add(socketList.Count);
            socketList.Add(new SocketData());
        }
        int id = GetFreeID();
        socketList[id].socket = client;
        socketList[id].stream = client.GetStream();
        socketList[id].setTime();
        socketUsedIDList.Add(id);

        network_data.set_ingame_param m = new set_ingame_param();
        m.set(id,0);
        m.shipchannel = 1;
        m.servertime = ServerTime.time;
        byte[] data = network_utils.nData.Instance.SerializeMsg<network_data.set_ingame_param>(m);
        Send(id, data);

        if (maxplayer == socketUsedIDList.Count)
        {
            beginning = true;
            Debug.Log("Begin Game");
            Console.WriteLine("Begin Game");
        }

        Debug.Log("Connect Game");
        Console.WriteLine("Connect Game");
        if (beginning == false)
            listener.BeginAcceptTcpClient(new AsyncCallback(Listen), listener);
    }

    void Receive(ref List<network_utils.cl_data> back)
    {
        for (int i = 0; i < socketUsedIDList.Count; i++)
        {
            try
            {
                while (socketList[socketUsedIDList[i]].stream.DataAvailable)
                {
                    byte[] recieve = new Byte[1024];
                    int len = socketList[socketUsedIDList[i]].stream.Read(recieve, 0, network_utils.nData.Instance.getSize<network_utils.HEADER>());
                    network_utils.HEADER header = network_utils.nData.Instance.DeserializeMsg<network_utils.HEADER>(recieve);
                    if (header.signum == network_utils.SIGNUM.BIN)
                    {
                        int restlaenge = header.size - network_utils.nData.Instance.getSize<network_utils.HEADER>();
                        if (restlaenge > 0)
                        {
                            len += socketList[socketUsedIDList[i]].stream.Read(recieve, network_utils.nData.Instance.getSize<network_utils.HEADER>(), restlaenge);
                        }
                    }
                    byte[] recieved = new Byte[len];
                    System.Array.Copy(recieve, 0, recieved, 0, len);
                    network_utils.cl_data cl = new network_utils.cl_data(socketUsedIDList[i], recieved);
                    cl.containerID = socketUsedIDList[i];
                    back.Add(cl);
                }
            }
            catch (Exception e)
            {
                SocketData s = socketList[socketUsedIDList[i]];
                socketList[socketUsedIDList[i]] = s;
            }
        }
    }
    bool IsConnected(Socket socket)
    {
        if (socket.Connected)
            return true;
        else
            return false;
    }
    public override bool Send(int contID, byte[] byteData)
    {
        if (socketList[contID].stream == null) return false; 
        try
        {
            if (socketList[contID].stream.CanWrite)
                socketList[contID].stream.Write(byteData, 0, byteData.Length);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
        return true;
    }
    void CheckPingOut()
    {
        for (int i = 0; i < socketUsedIDList.Count; i++)
        {
            TimeSpan duration = DateTime.Now - socketList[socketUsedIDList[i]].time;
            if (duration > TimeSpan.FromSeconds(10))
            {
                SocketData s = socketList[socketUsedIDList[i]];
                s.del = true;
                socketList[socketUsedIDList[i]] = s;
            }
        }
    }
    void checkDel()
    {
        List<int> delList = new List<int>();
        for (int i = 0; i < socketUsedIDList.Count; i++)
        {
            if (socketList[socketUsedIDList[i]].del || !socketList[socketUsedIDList[i]].socket.Connected)
            {
                socketList[socketUsedIDList[i]].del = false;
                delList.Add(i);
            }
        }
        for (int i = 0; i < delList.Count; i++)
        {
            int id = socketUsedIDList[delList[i]];
            SocketData so = socketList[id];
            AddFreeID(id);
            Debug.Log("Disconnect from Game " + id);
            while(so.channels.Count > 0)
            {
                channel c = ChannelObjectList[so.channels[0]].GetComponent<channel>();
                if(c != null)
                {
                    c.UnregisterEntity(id);
                }
            }
        }
    }
    void SetPing(int contID)
    {
        socketList[contID].setTime();
    }
    public override void RegisterChannelToSocketdata(int contID, int channel)
    {
        socketList[contID].channels.Add(channel);
    }
    public override void UnregisterChannelToSocketdata(int contID, int channel)
    {
        socketList[contID].channels.Remove(channel);
    }
}
