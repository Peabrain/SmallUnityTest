using UnityEngine;
using System.Collections;

public class channel : MonoBehaviour
{

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
}
