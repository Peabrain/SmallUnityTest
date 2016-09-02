using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public GameObject door = null;
	// Use this for initialization
	void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enterd");
        if (door != null)
        {
            Debug.Log("Play Anim");
            if(!door.GetComponent<Animation>().isPlaying)
                door.GetComponent<Animation>()["open"].normalizedTime = 0;
            door.GetComponent<Animation>()["open"].speed = 1;
            door.GetComponent<Animation>().Play("open");
        }
    }
    void OnTriggerStay(Collider other)
    {
//        Debug.Log("innen");
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        if (door != null)
        {
            Debug.Log("Play Anim");
            if (!door.GetComponent<Animation>().isPlaying)
                door.GetComponent<Animation>()["open"].normalizedTime = 1;
            door.GetComponent<Animation>()["open"].speed = -1;
            door.GetComponent<Animation>().Play("open");
        }
    }
}
