using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class interactionlink : MonoBehaviour {

    [Serializable]
    public struct triggerObj
    {
        public int id;
        public trigger tr;
    }
    public triggerObj[] tri;

    public int netID = 0;
    Dictionary<int, trigger> triggers = new Dictionary<int, trigger>();
    bool active = false;
	// Use this for initialization
	void Start () {
    }

    public void Init(bool isClient)
    {
        if (isClient)
        {
            foreach (triggerObj t in tri)
            {
                t.tr.GetComponent<BoxCollider>().enabled = false;
                triggers[t.id] = t.tr;
            }
        }
        else
        {
            foreach (triggerObj t in tri)
            {
                triggers[t.id] = t.tr;
            }
        }
    }
    // Update is called once per frame
    void Update () {
    }

    public virtual void Logic(channel mychannel)
    {
        foreach (KeyValuePair<int, trigger> j in triggers)
        {
            trigger t = (trigger)j.Value;
            if (t.HasChanged() == true)
            {
                if (t.IsOn())
                    Activate();
                else
                    Deactivate();
                t.ClearChanged();
                network_data.trigger m1 = new network_data.trigger();
                m1.set(-1, mychannel.GetChannel());
                m1.netID = netID;
                m1.on = t.IsOn();
                byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.trigger>(m1);
                mychannel.SendToChannel(ref data1);
            }
        }
    }
    public virtual void Activate()
    {
        if (!GetComponent<Animation>().isPlaying)
            GetComponent<Animation>()["open"].normalizedTime = 0;
        GetComponent<Animation>()["open"].speed = 1;
        GetComponent<Animation>().Play("open");
    }
    public virtual void Deactivate()
    {
        if (!GetComponent<Animation>().isPlaying)
            GetComponent<Animation>()["open"].normalizedTime = 1;
        GetComponent<Animation>()["open"].speed = -1;
        GetComponent<Animation>().Play("open");
    }
}
