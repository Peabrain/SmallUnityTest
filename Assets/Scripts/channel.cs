using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class channel : MonoBehaviour
{

    Dictionary<int, GameObject> entities = new Dictionary<int, GameObject>();
    public network mynetwork = null;
    public int number = -1;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void ProcessMessage(ref byte[] message)
    {
    }
    public void SetChannel(int ch)
    {
        number = ch;
    }
    public void SetNetwork(network n)
    {
        mynetwork = n;
    }
    public bool SendToChannel(ref byte[] byteData)
    {
        foreach(KeyValuePair<int,GameObject> i in entities)
        {
            mynetwork.Send(i.Key,byteData);
        }
        return true;
    }
    internal void RegisterEntity(GameObject g, int contID)
    {
        entities[contID] = g;
        mynetwork.RegisterChannelToSocketdata(contID, number);
    }
    internal virtual void UnregisterEntity(int contID)
    {
        if(entities.ContainsKey(contID))
        {
            entities.Remove(contID);
        }
        mynetwork.UnregisterChannelToSocketdata(contID, number);
    }
    internal GameObject GetEntity(int contID)
    {
        if (entities.ContainsKey(contID))
            return entities[contID];
        return null;
    }
    internal IDictionaryEnumerator FirstEntity()
    {
        return entities.GetEnumerator();
    }
}
