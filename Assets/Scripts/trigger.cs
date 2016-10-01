using UnityEngine;
using System.Collections;

public class trigger : MonoBehaviour {

    bool mouseover = false;
    channel mychannel = null;
    int counter = 0;
    bool on = false;
    bool changed = false;
    public int netID = 0;
    public bool auto = false;
    public GameObject InteractionLink = null;
    interactionlink link = null;
	// Use this for initialization
    public void Init(channel mychannel)
    {
        this.mychannel = mychannel;
        if (InteractionLink)
        {
            link = InteractionLink.GetComponent<interactionlink>();
            if (link)
                link.SetTrigger(this);
        }
    }
	void OnTriggerEnter(Collider other)
    {
        if (counter == 0)
        {
            on = true;
            changed = true;
        }
        counter++;
        if (!mychannel.GetNetwork().IsClient()) SendTrigger(-1);
    }
    void OnTriggerStay(Collider other)
    {
//        Debug.Log("innen");
    }
    void OnTriggerExit(Collider other)
    {
        counter--;
        if (counter == 0)
        {
            on = false;
            changed = true;
        }
        if (!mychannel.GetNetwork().IsClient()) SendTrigger(-1);
    }
    void OnMouseOver()
    {
        mouseover = true;
    }
    void OnMouseExit()
    {
        mouseover = false;
    }
    public bool MouseOver()
    {
        return mouseover;
    }
    public void SetTrigger(int count,bool on)
    {
//        Debug.Log("Trigger " + name + " " + on);
        counter = count;
        if (this.on ^ on)
            changed = true;
        this.on = on;
    }
    void SendTrigger(int contID)
    {
        bool accept = link.Accept(on, contID);
        SendTrigger(contID,accept);
    }
    void SendTrigger(int contID,bool accept)
    {
        network_data.trigger m1 = new network_data.trigger();
        m1.set(contID, mychannel.GetChannel());
        m1.netID = netID;
        m1.count = counter;
        m1.on = on;
        m1.accept = accept;
        byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.trigger>(m1);
        mychannel.SendToChannel(ref data1);
        if (!mychannel.GetNetwork().IsClient() && m1.accept)
        {
            DoActivate(contID);
        }
    }
    public void SendTriggerTo(int contID)
    {
        network_data.trigger m1 = new network_data.trigger();
        m1.set(-1, mychannel.GetChannel());
        m1.netID = netID;
        m1.count = counter;
        m1.on = on;
        m1.accept = link.Accept(on,contID);
        byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.trigger>(m1);
        mychannel.GetNetwork().Send(contID, data1);
    }
    public void SendRequest(int contID)
    {
        if (mychannel.GetNetwork().IsClient())
        {
            network_data.trigger m1 = new network_data.trigger();
            m1.set(contID, mychannel.GetChannel());
            m1.netID = netID;
            m1.count = counter;
            m1.on = on;
            byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.trigger>(m1);
            mychannel.GetNetwork().Send(contID, data1);
        }
    }
    public void TriggerRequest(int contID)
    {
        Debug.Log("Get Request from " + contID);
        bool on_ = this.on ^ true;
        bool accept = link.Accept(on_, contID);
        if (accept)
        {
            if (on_)
                counter = 1;
            else
                counter = 0;
            SetTrigger(counter, on_);
        }
        SendTrigger(contID,accept);
    }
    public void DoActivate(int contID)
    {
        if (changed)
        {
            changed = false;
            if (on)
                link.Activate(contID);
            else
                link.Deactivate(contID);
        }
    }
    public interactionlink GetLink()
    {
        return link;
    }
    public channel GetChannel()
    {
        return mychannel;
    }
}
