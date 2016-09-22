using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class interactionlink : MonoBehaviour {

    public int netID = 0;
    public GameObject TriggerObject = null;
    trigger Trigger = null;
    channel mychannel = null;
    // Use this for initialization
    void Start () {
    }

    public void Init(channel mychannel)
    {
        this.mychannel = mychannel;
        Trigger = TriggerObject.GetComponent<trigger>();
        if (mychannel.GetNetwork().IsClient())
        {
            if (Trigger.auto) Trigger.GetComponent<BoxCollider>().enabled = false;
        }
    }
    // Update is called once per frame
    void Update () {
        if(mychannel != null)
        {
            if (!mychannel.GetNetwork().IsClient())
            {
                if (Trigger.HasChanged() == true)
                {
                    if (Trigger.IsOn())
                        Activate();
                    else
                        Deactivate();
                    Trigger.ClearChanged();
                    network_data.trigger m1 = new network_data.trigger();
                    m1.set(-1, mychannel.GetChannel());
                    m1.netID = netID;
                    m1.on = Trigger.IsOn();
                    byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.trigger>(m1);
                    mychannel.SendToChannel(ref data1);
                }
            }
        }
    }

    public virtual void Logic(channel mychannel)
    {
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
