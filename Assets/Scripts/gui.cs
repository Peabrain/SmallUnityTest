using UnityEngine;
using System.Collections;

public class gui : MonoBehaviour {

    // Use this for initialization
    private bool isAtStartup = true;
    void Awake()
    {
    }
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isAtStartup)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                var go = new GameObject();
                go.name = "game";
                go.AddComponent<game>();
                go.AddComponent<server>();
                network n = go.GetComponent<network>();
                n.AddGameObjectToChannel(go);
                go.GetComponent<game>().LoadShip("Prefabs/Ship1", "Ship");
                isAtStartup = false;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                var go = new GameObject();
                go.name = "game";
                go.AddComponent<game>();
                go.AddComponent<client>();
                network n = go.GetComponent<network>();
                n.AddGameObjectToChannel(go);
                go.GetComponent<game>().LoadShip("Prefabs/Ship1", "Ship");
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
