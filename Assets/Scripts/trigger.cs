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
        if (InteractionLink) link = InteractionLink.GetComponent<interactionlink>();
    }
	void OnTriggerEnter(Collider other)
    {
        if (counter == 0)
        {
            on = true;
            changed = true;
        }
        counter++;
        if (!mychannel.GetNetwork().IsClient()) SendTrigger();
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
        if (!mychannel.GetNetwork().IsClient()) SendTrigger();
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
        counter = count;
        if (this.on ^ on)
            changed = true;
        this.on = on;
    }
    void SendTrigger()
    {
        network_data.trigger m1 = new network_data.trigger();
        m1.set(-1, mychannel.GetChannel());
        m1.netID = netID;
        m1.count = counter;
        m1.on = on;
        byte[] data1 = network_utils.nData.Instance.SerializeMsg<network_data.trigger>(m1);
        mychannel.SendToChannel(ref data1);
        Debug.Log("trigger " + name + " " + counter);
        if (!mychannel.GetNetwork().IsClient())
        {
            DoActivate();
        }
    }
    public void SendTriggerTo(int contID)
    {
        network_data.trigger m1 = new network_data.trigger();
        m1.set(-1, mychannel.GetChannel());
        m1.netID = netID;
        m1.count = counter;
        m1.on = on;
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
    public void TriggerRequest()
    {
        bool on_ = this.on ^ true;
        if (on_)
            counter = 1;
        else
            counter = 0;
        SetTrigger(counter,on_);
        if (changed)
            SendTrigger();
    }
    public void DoActivate()
    {
        if (changed)
        {
            changed = false;
            if (on)
                link.Activate();
            else
                link.Deactivate();
        }
    }
}
