using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace network_utils
{

    public enum SIGNUM
    {
        BIN = 0x7f2399bc
    };
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HEADER
    {
        public SIGNUM signum
        {
            get { return Signum; }
            set { Signum = value; }
        }
        public int containerID
        {
            get { return ContainerID; }
            set { ContainerID = value; }
        }
        public int channelID
        {
            get { return ChannelID; }
            set { ChannelID = value; }
        }
        public int command
        {
            get { return Command; }
            set { Command = value; }
        }
        public int size
        {
            get { return Size; }
            set { Size = value; }
        }

        private SIGNUM Signum;
        private int Command;
        private int ContainerID;
        private int ChannelID;
        private int Size;
    }

    class nData
    {
        static nData instance = null;
        static readonly object padlock = new object();
        public int containerID = -1;

        nData()
        {
        }
        public static nData Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new nData();
                    }
                    return instance;
                }
            }
        }

        public int getSize<T>() where T : struct
        {
            int objsize = Marshal.SizeOf(typeof(T));
            return objsize;
        }
        public Byte[] SerializeMsg<T>(T msg) where T : struct
        {
            int objsize = Marshal.SizeOf(typeof(T));
            Byte[] ret = new Byte[objsize];

            IntPtr buff = Marshal.AllocHGlobal(objsize);

            Marshal.StructureToPtr(msg, buff, true);

            Marshal.Copy(buff, ret, 0, objsize);

            Marshal.FreeHGlobal(buff);

            return ret;
        }
        public T DeserializeMsg<T>(Byte[] data) where T : struct
        {
            int objsize = Marshal.SizeOf(typeof(T));
            IntPtr buff = Marshal.AllocHGlobal(objsize);

            Marshal.Copy(data, 0, buff, objsize);

            T retStruct = (T)Marshal.PtrToStructure(buff, typeof(T));

            Marshal.FreeHGlobal(buff);

            return retStruct;
        }
    }
    public class cl_data
    {
        public int containerID;
        public byte[] data;
        public cl_data(int contID, byte[] da)
        {
            containerID = contID;
            data = new byte[da.Length];
            da.CopyTo(data, 0);
        }
    }
}
