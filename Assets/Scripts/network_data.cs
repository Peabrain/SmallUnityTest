using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
//using network_lobby;
using network_utils;

namespace network_data
{  
	public enum COMMANDS { 
//        clogin_ask = 1,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
//        clogin_anser = 2, 
        cping = 3,
//        center_lobby = 4,
//        cleave_lobby = 5,
//        csesson_create = 6,
//        csesson_delete = 7,
//        csesson_enter = 8,
//        cstart_session = 9,
//        cstarted_session = 10,
//		cend_ingame = 11,
        cset_ingame_param = 12,
        center_ship = 13,
        ccreate_player = 14,
        cmove_player = 15,
        cdisconnect = 16,
        ctrigger = 17
    };

/*    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct login_ask
    {
        public void set(int contID,int channel, string name, string password)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.clogin_ask;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.size = network_utils.nData.Instance.getSize<login_ask>();
            header = h;

            this.name = name;
            this.password = password;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public string name
        {
            get { return Name; }
            set { Name = value; }
        }
        public string password
        {
            get { return Password; }
            set { Password = value; }
        }
        private HEADER Header;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        string Name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        string Password;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct login_anser
    {
        public void set(int contID, bool logged_in)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.clogin_anser;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.size = network_utils.nData.Instance.getSize<login_anser>();
            header = h;
            this.logged_in = logged_in;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public bool logged_in
        {
            get { return Logged_in; }
            set { Logged_in = value; }
        }
        private HEADER Header;
        private bool Logged_in;
    }
*/
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ping
    {
        public void set(int contID, int channel)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.cping;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.channelID = channel;
            h.size = network_utils.nData.Instance.getSize<ping>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        private HEADER Header;
    }
/*    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct enter_lobby
    {
        public void set(int contID)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.center_lobby;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.size = network_utils.nData.Instance.getSize<enter_lobby>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        private HEADER Header;
    }
    public struct leave_lobby
    {
        public void set(int contID)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.cleave_lobby;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.size = network_utils.nData.Instance.getSize<leave_lobby>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        private HEADER Header;
    }
/*        [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct session_create
    {
        public void set(int contID,network_lobby.SESSION_DATA data)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.csesson_create;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.size = network_utils.nData.Instance.getSize<session_create>();
            header = h;
            this.data = data;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public network_lobby.SESSION_DATA data
        {
            get { return Data; }
            set { Data = value; }
        }
        private HEADER Header;
        private network_lobby.SESSION_DATA Data;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct session_delete
    {
        public void set(int contID)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.csesson_delete;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.size = network_utils.nData.Instance.getSize<session_delete>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        HEADER Header;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct session_enter
    {
        public void set(int contID, int team)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.csesson_enter;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.size = network_utils.nData.Instance.getSize<session_enter>();
            header = h;
            this.team = team;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public int contID
        {
            get { return ContID; }
            set { ContID = value; }
        }
        public int team
        {
            get { return Team; }
            set { Team = value; }
        }
        private HEADER Header;
        private int ContID;
		private int Team;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct session_start
    {
        public void set(int contID, int port,int ki,int player_id,string servername)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.cstart_session;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.size = network_utils.nData.Instance.getSize<session_start>();
            header = h;
            this.session_port = port;
            this.ki = ki;
            this.player_id = player_id;
            this.servername = servername;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public int session_port
        {
            get { return Session_port; }
            set { Session_port = value; }
        }
        public int ki
        {
            get { return Ki; }
            set { Ki = value; }
        }
        public int team
        {
            get { return Team; }
            set { Team = value; }
        }
        public int player_id
        {
            get { return Player_id; }
            set { Player_id = value; }
        }
        public string servername
        {
            get { return Servername; }
            set { Servername = value; }
        }
        private HEADER Header;
        private int Session_port;
        private int Ki;
		private int Team;
		private int Player_id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private string Servername;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct end_ingame
    {
        public void set(int contID)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.cend_ingame;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.size = network_utils.nData.Instance.getSize<session_start>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        private HEADER Header;
    }
*/
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct set_ingame_param
    {
        public void set(int contID, int channel)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.cset_ingame_param;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.channelID = channel;
            h.size = network_utils.nData.Instance.getSize<set_ingame_param>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public string playername
        {
            get { return Playername; }
            set { Playername = value; }
        }
        public int shipchannel
        {
            get { return ShipChannel; }
            set { ShipChannel = value; }
        }
        private HEADER Header;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private string Playername;
        private int ShipChannel;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct enter_ship
    {
        public void set(int contID, int channel)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.center_ship;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.channelID = channel;
            h.size = network_utils.nData.Instance.getSize<enter_ship>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public int channel
        {
            get { return Channel; }
            set { Channel = value; }
        }
        public int shipchannel
        {
            get { return ShipChannel; }
            set { ShipChannel = value; }
        }
        private HEADER Header;
        private int Channel;
        private int ShipChannel;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct create_player
    {
        public void set(int contID, int channel)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.ccreate_player;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.channelID = channel;
            h.size = network_utils.nData.Instance.getSize<create_player>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public string playername
        {
            get { return Playername; }
            set { Playername = value; }
        }
        public Vector3 position
        {
            get { return Position; }
            set { Position = value; }
        }
        public Quaternion rotation
        {
            get { return Rotation; }
            set { Rotation = value; }
        }
        private HEADER Header;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private string Playername;
        private Vector3 Position;
        private Quaternion Rotation;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct move_player
    {
        public void set(int contID, int channel)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.cmove_player;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.channelID = channel;
            h.size = network_utils.nData.Instance.getSize<move_player>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public Vector3 position
        {
            get { return Position; }
            set { Position = value; }
        }
        public Quaternion rotation
        {
            get { return Rotation; }
            set { Rotation = value; }
        }
        public Vector3 velocity
        {
            get { return Velocity; }
            set { Velocity = value; }
        }
        private HEADER Header;
        private Vector3 Position;
        private Quaternion Rotation;
        private Vector3 Velocity;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct disconnect
    {
        public void set(int contID, int channel)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.cdisconnect;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.channelID = channel;
            h.size = network_utils.nData.Instance.getSize<disconnect>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        private HEADER Header;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct trigger
    {
        public void set(int contID, int channel)
        {
            HEADER h = new HEADER();
            h.command = (int)COMMANDS.ctrigger;
            h.signum = SIGNUM.BIN;
            h.containerID = contID;
            h.channelID = channel;
            h.size = network_utils.nData.Instance.getSize<trigger>();
            header = h;
        }
        public HEADER header
        {
            get { return Header; }
            set { Header = value; }
        }
        public int count
        {
            get { return Count; }
            set { Count = value; }
        }
        public int netID
        {
            get { return NetID; }
            set { NetID = value; }
        }
        public bool on
        {
            get { return On; }
            set { On = value; }
        }
        public bool accept
        {
            get { return Accept; }
            set { Accept = value; }
        }
        private HEADER Header;
        private int Count;
        private bool On;
        private bool Accept;
        private int NetID;
    }
}
