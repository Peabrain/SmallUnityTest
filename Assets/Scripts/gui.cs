using UnityEngine;
using System.Collections;

public class gui : MonoBehaviour {

    // Use this for initialization
    private bool isAtStartup = true;
    public GameObject network = null;
    public GameObject game = null;
    void Awake()
    {
    }
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isAtStartup && network != null)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                var go = new GameObject();
                go.name = "networking";
                go.AddComponent<server>();
                isAtStartup = false;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                var go = new GameObject();
                go.name = "networking";
                go.AddComponent<client>();
                isAtStartup = false;
            }
        }
    }
    void OnGUI()
    {
        if (isAtStartup)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Press S for server");
            //            GUI.Label(new Rect(2, 30, 150, 100), "Press B for both");
            GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
        }
  //      else
 //           network.GetComponent<network>().OnGUI();
    }
}
