using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class channel : MonoBehaviour
{

    Dictionary<int, GameObject> entities = new Dictionary<int, GameObject>();
    network mynetwork = null;
    int number = -1;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ProcessMessage(ref byte[] message,receiver rec)
    {
        if (rec) rec.ProcessMessage(ref message);
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
        Debug.Log("Channel: " + number + ", register entry: " + contID + " (" + g.name + ")");
    }
    internal virtual void UnregisterEntity(int contID)
    {
        if(entities.ContainsKey(contID))
        {
            GameObject g = entities[contID];
            entities.Remove(contID);
            Debug.Log("Channel: " + number + ", unregister entry: " + contID + " (" + g.name + ")");
        }
        mynetwork.UnregisterChannelToSocketdata(contID, number);
    }
    internal GameObject GetEntity(int contID)
    {
        if (entities.ContainsKey(contID))
            return entities[contID];
        else
            Debug.Log("Channel: " + number + ", entry: " + contID + " not found");
        return null;
    }
    internal IDictionaryEnumerator FirstEntity()
    {
        return entities.GetEnumerator();
    }
    public int GetChannel()
    {
        return number;
    }
    public network GetNetwork()
    {
        return mynetwork;
    }
}
