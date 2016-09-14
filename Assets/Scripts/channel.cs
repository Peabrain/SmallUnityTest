using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class channel : MonoBehaviour
{

    internal Dictionary<int, GameObject> entity = new Dictionary<int, GameObject>();
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
        foreach(KeyValuePair<int,GameObject> i in entity)
        {
            mynetwork.Send(i.Key,byteData);
        }
        return true;
    }
}
