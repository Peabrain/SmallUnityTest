using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Text;

public class network : MonoBehaviour {

    public System.Diagnostics.Stopwatch Timer = System.Diagnostics.Stopwatch.StartNew();

    public class SocketData
    {
        public SocketData()
        {
            del = false;
            time = new DateTime();
            this.setTime();
        }
        public void setTime()
        {
            time = DateTime.Now;
        }
        public NetworkStream stream;
        public TcpClient socket;
        public DateTime time;
        public bool del;
        public string playername;
        public List<int> channels = new List<int>();
    };

    internal Dictionary<int, GameObject> ChannelObjectList = new Dictionary<int, GameObject>();

    public int AddGameObjectToChannel(GameObject g,int c)
    {
        ChannelObjectList[c] = g;
        return c;
    }

    public virtual bool Send(int contID, byte[] byteData)
    {
        return false;
    }
    public virtual bool IsClient()
    {
        return false;
    }
    public virtual void RegisterChannelToSocketdata(int contID, int channel)
    {

    }
    public virtual void UnregisterChannelToSocketdata(int contID, int channel)
    {

    }
}
