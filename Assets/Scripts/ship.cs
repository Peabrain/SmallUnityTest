using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class ship : puppet {

    public int netID = 0;
    public List<GameObject> Spawnpoints = new List<GameObject>();
    int spos = 0;
    Dictionary<int, trigger> Triggers = new Dictionary<int, trigger>();
    DateTime last_clientupdate = new DateTime();
    GameObject myCam = null;
    channel Channel = null;

    GameObject Game = null;

    Vector3 g = new Vector3(0, 0, 0);

    public float movementSpeed = 4.0f;
    public float Gravity = 10.0f;
    internal bool grounded = false;

    Rigidbody myrigidbody = null;

    Vector3 targetVelocity = new Vector3(0, 0, 0);
//    Quaternion targetRotation = new Quaternion();

    ilink_cockpit [] cockpits = null;

    // Use this for initialization
    public void Init(GameObject Game)
    {
//        targetRotation = Quaternion.Euler(0,0,0);
        myrigidbody = transform.FindChild("Outside").GetComponent<Rigidbody>();
        this.Game = Game;

        cockpits = transform.GetComponentsInChildren<ilink_cockpit>();
        Debug.Log("found " + cockpits.Length + " cockpits");
        Channel = this.GetComponent<channel>();
        if (!Channel)
            Debug.Log("No Channel");
        else
        {
            if (!Channel.GetNetwork())
                Debug.Log("No Network for Channel");
        }
        trigger[] trig = transform.GetComponentsInChildren<trigger>();
        foreach (trigger ti in trig)
        {
            ti.Init(Channel);
            if (Triggers.ContainsKey(ti.netID))
                Debug.Log("Triggerkey " + ti.netID + "error");
            Triggers[ti.netID] = ti;
            if (Channel.GetNetwork().IsClient())
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
    void Start ()
    {
    }

    public channel GetChannel()
    {
        return Channel;
    }
    // Update is called once per frame
    void Update()
    {
        if (!Channel || !Channel.GetNetwork()) return;

        if(!Channel.GetNetwork().IsClient())
        {
            InterpolateMovement(puppet.trans_flag_rotation);
            transform.localPosition += myrigidbody.gameObject.transform.localPosition;
            myrigidbody.gameObject.transform.localPosition = Vector3.zero;
//            transform.localRotation *= myrigidbody.gameObject.transform.localRotation;
//            myrigidbody.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            if(!IsCockpitUsedByMe())
                InterpolateMovement(puppet.trans_flag_position | puppet.trans_flag_rotation);
            else
            {
                InterpolateMovement(puppet.trans_flag_position);
                transform.localRotation *= myrigidbody.gameObject.transform.localRotation;
                myrigidbody.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        DateTime now = new DateTime();
        now = DateTime.Now;
        TimeSpan duration = now - last_clientupdate;

        if (Channel.GetNetwork().IsClient())
        {
            int id = Channel.GetNetwork().GetComponent<client>().ingameContID;
            GameObject g = Channel.GetEntity(id);

            if (duration > TimeSpan.FromMilliseconds(100) && g != null)
            {
                last_clientupdate = now;
                network_data.move_player m = new network_data.move_player();
                m.set(id, Channel.GetChannel());
                m.position = g.transform.localPosition;
                m.rotation = g.transform.localRotation;
                byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.move_player>(m);
                Channel.GetNetwork().Send(id, data1);

                if (IsCockpitUsedByMe())
                {
                    network_data.move_player m2 = new network_data.move_player();
                    m2.set(GetComponent<channel>().GetChannel(), Game.GetComponent<channel>().GetChannel());
                    m2.position = transform.localPosition;
                    m2.rotation = transform.localRotation;
                    m2.velocity = targetVelocity;
                    byte[] data = network_utils.nData.Instance.SerializeMsg<network_data.move_player>(m2);
                    Game.GetComponent<channel>().GetNetwork().Send(GetComponent<channel>().GetChannel(), data);
                }
            }
        }
        else
        {
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
    }
    GameObject GetNextSpawnPoint()
    {
        int s = spos;
        spos = (spos + 1) % 2;
        return Spawnpoints[s];
    }
    public void SpawnPlayer(int contID,bool IsClient,bool ownPlayer,Vector3 position,Quaternion rotation)
    {
        Vector3 re = rotation.eulerAngles;
        GameObject Player = Instantiate(Resources.Load("Prefabs/Player", typeof(GameObject)),transform) as GameObject;
        Player.transform.localPosition = position;
        Player.transform.localRotation = rotation;
        Channel.RegisterEntity(Player, contID);
        if(IsClient && ownPlayer)
        {
            Transform t = Player.transform.FindChild("Camera");
            myCam = t.gameObject;
            t.gameObject.SetActive(true);
            Player.AddComponent<puppetcontrol>();
            Player.GetComponent<puppet>().InitTransform(position, rotation);// rotation);
            puppetcontrol f = Player.GetComponent<puppetcontrol>();
            f.SetChannel(Channel);
//            f.AddObjectToInteract(this.gameObject);
            GameObject GUI = GameObject.Find("GUI");
            if (GUI != null)
            {
                GUI.GetComponent<gui>().PushUserInterface(Player);
            }
        }
        else
        {
            Player.AddComponent<puppet>();
            Player.GetComponent<puppet>().InitTransform(position, rotation);// rotation);
        }
        Player.GetComponent<puppet>().SetTransform(position, rotation,puppet.trans_flag_position | puppet.trans_flag_rotation);
        if (Player) Debug.Log("Enter Ship (" + Channel.GetChannel() + ")" + " player " + contID + " pos: " + position + " rot:" + re.x + "," + re.y + "," + re.z);

        if(IsClient == false)
        {
            network_data.create_player m = new network_data.create_player();
            m.set(contID, Channel.GetChannel());
            m.position = position;
            m.rotation = rotation;
            byte[] data = network_utils.nData.Instance.SerializeMsg<network_data.create_player>(m);
            Channel.SendToChannel(ref data);

            List<byte[]> datalist = new List<byte[]>();
            IDictionaryEnumerator r = Channel.FirstEntity();
            while(r.MoveNext())
            {
                if (contID != (int)r.Key)
                {
                    m.set((int)r.Key, Channel.GetChannel());
                    m.position = ((GameObject)r.Value).transform.localPosition;
                    m.rotation = ((GameObject)r.Value).transform.localRotation;
                    byte[]data1 = network_utils.nData.Instance.SerializeMsg<network_data.create_player>(m);
                    Channel.GetNetwork().Send(contID, data1);
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
                    if (Channel.GetNetwork().IsClient() == false)
                    {
                        GameObject g = GetNextSpawnPoint();
                        SpawnPlayer(com.header.containerID, Channel.GetNetwork().IsClient(), false, g.transform.localPosition, g.transform.localRotation);
                    }
                }
                break;
            case (int)network_data.COMMANDS.ccreate_player:
                {
                    network_data.create_player com = network_utils.nData.Instance.DeserializeMsg<network_data.create_player>(message);
                    if (Channel.GetNetwork().IsClient() == true)
                    {
                        SpawnPlayer(com.header.containerID, Channel.GetNetwork().IsClient(), Channel.GetNetwork().GetComponent<client>().ingameContID == com.header.containerID, com.position, com.rotation);
                    }
                }
                break;
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
                                g.GetComponent<puppet>().SetTransform(com.position, com.rotation, puppet.trans_flag_position | puppet.trans_flag_rotation);
                            }
                        }
                    }
                    else
                    {
                        GameObject g = Channel.GetEntity(com.header.containerID);
                        if (g != null)
                        {
                            g.GetComponent<puppet>().SetTransform(com.position,com.rotation, puppet.trans_flag_position | puppet.trans_flag_rotation);
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
                    if(Triggers.ContainsKey(com.netID))
                    {
                        trigger t = Triggers[com.netID];
                        if (Channel.GetNetwork().IsClient())
                        {
                            if(com.accept)
                            {
                                t.SetTrigger(com.count, com.on);
                                t.GetLink().Accept(com.on, com.header.containerID);
                                t.DoActivate(com.header.containerID);
                            }
                        }
                        else
                        {
                            t.TriggerRequest(com.header.containerID);
                            t.DoActivate(com.header.containerID);
                        }
                    }
                }
                break;
        }
    }
    void UnregisterEntity(int contID)
    {
        GameObject g = Channel.GetEntity(contID);
        Channel.UnregisterEntity(contID);
        if (g != null)
        {
            Destroy(g);
        }
        if (Channel.GetNetwork().IsClient() == false)
        {
            network_data.disconnect m = new network_data.disconnect();
            m.set(contID, Channel.GetChannel());
            byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.disconnect>(m);
            Channel.SendToChannel(ref data1);
        }
    }
/*    public trigger MouseOver()
    {
        trigger ret = null;

        if (Channel.GetNetwork().IsClient())
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
                            Collider[] c = k.GetComponents<Collider>();
                            ret = k;
                            Vector3 v = myCam.transform.rotation * new Vector3(0,0,1);
                            Ray ray = myCam.GetComponent<Camera>().ScreenPointToRay(new Vector3(0, 0, 0));
                            ray.direction = v;
                            ray.origin = myCam.transform.position;
                            RaycastHit hitinfo;
                            float dist = 1.5f;
                            foreach (Collider cs in c)
                            {
                                if (cs.Raycast(ray, out hitinfo, dist))
                                {
                                    ret = k;
                                    dist = hitinfo.distance;
                                }
                            }
                        }
                    }
                }
            }
        }
        return ret;
    }
*/
    void FixedUpdate()
    {
        if (Channel.GetNetwork().IsClient()) return;

//        transform.rotation = targetRotation;
        Vector3 t = myrigidbody.gameObject.transform.rotation * targetVelocity;
//        t = myrigidbody.gameObject.transform.rotation * t;
        Vector3 velocity = myrigidbody.velocity;
        Vector3 velocityChange = t - velocity;
        myrigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        grounded = false;
    }
    public void SetVelocity(Vector3 v)
    {
        targetVelocity = v;
    }
    public Vector3 GetVelocity()
    {
        return targetVelocity;
    }
    /*    public void SetTargetRotation(Quaternion v)
            {
                targetRotation = v;
            }
            public Quaternion GetTargetRotation()
            {
                return targetRotation;
            }
        */
    void OnCollisionStay()
    {
        grounded = true;
    }
    public bool IsCockpitUsedByMe()
    {
        if(cockpits != null && Channel.GetNetwork().IsClient())
        {
            client cl = (client)Channel.GetNetwork();
            int contID = cl.ingameContID;
            foreach(ilink_cockpit c in cockpits)
            {
                if(c.GetUserID() == contID)
                {
                    return true;
                }
            }
        }
        return false;
    }
    /*    public void SetTransform(Vector3 v, Quaternion r)
        {
            syncTime = 0f;
            syncDelay = Time.time - lastTime;
            lastTime = Time.time;

            lastPos = transform.localPosition;
            destPos = v;
            lastRot = transform.localRotation;
            destRot = r;
        }
    */
}
