using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gui : MonoBehaviour {

    // Use this for initialization
    private bool isAtStartup = true;
    bool isClient = false;
    public string myserver;
    List<GameObject> userInterfaceList = new List<GameObject>();
    GameObject game = null;
    
    void Awake()
    {
        string[] sys = System.Environment.GetCommandLineArgs();
        foreach(string s in sys)
        {
            if (s == "-batchmode")
            {
                game = new GameObject();
                game.name = "game";
                game.AddComponent<game>();
                channel ss = game.AddComponent<channel>();
                game.AddComponent<server>();
                network n = game.GetComponent<network>();
                int t = n.AddGameObjectToChannel(game);
                ss.SetNetwork(n);
                ss.SetChannel(t);
                game.GetComponent<game>().Init();
                game.GetComponent<game>().LoadShip("Prefabs/Ship1", "Ship");
                isAtStartup = false;
                break;
            }
        }
    }
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (isAtStartup)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                game = new GameObject();
                game.name = "game";
                game.AddComponent<game>();
                channel s = game.AddComponent<channel>();
                game.AddComponent<server>();
                network n = game.GetComponent<network>();
                int t = n.AddGameObjectToChannel(game);
                s.SetNetwork(n);
                s.SetChannel(t);
                game.GetComponent<game>().Init();
                game.GetComponent<game>().LoadShip("Prefabs/Ship1", "Ship");
                isAtStartup = false;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                isClient = true;
                game = new GameObject();
                game.name = "game";
                game.AddComponent<game>();
                channel s = game.AddComponent<channel>();
                game.AddComponent<client>();
                network n = game.GetComponent<network>();
                int t = n.AddGameObjectToChannel(game);
                s.SetNetwork(n);
                s.SetChannel(t);
                game.GetComponent<game>().Init();
                game.GetComponent<client>().Connect(myserver);
                game.GetComponent<game>().LoadShip("Prefabs/Ship1", "Ship");
                isAtStartup = false;
                Cursor.lockState = CursorLockMode.Locked;
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
            myserver = GUI.TextField(new Rect(2, 90, 150, 20), myserver,25);
        }
        //      else
        //           network.GetComponent<network>().OnGUI();
    }
    public bool PushUserInterface(GameObject obj)
    {
        if (!isClient) return true;
        if (!userInterfaceList.Contains(obj))
        {
            puppet ui = obj.GetComponent<puppet>();
            if (ui)
            {
                if (userInterfaceList.Count != 0)
                {
                    GameObject obj_last = userInterfaceList[userInterfaceList.Count - 1];
                    if (obj_last)
                    {
                        puppet ui_last = obj_last.GetComponent<puppet>();
                        if (ui_last)
                            ui_last.SetActiv(false);
                    }
                }
                ui.SetActiv(true);
                userInterfaceList.Add(obj);
                return true;
            }
        }
        return false;
    }
    public void PopUserInterface()
    {
        if (!isClient) return;
        if (userInterfaceList.Count > 0)
        {
            GameObject obj_last = userInterfaceList[userInterfaceList.Count - 1];
            if (obj_last)
            {
                puppet ui_last = obj_last.GetComponent<puppet>();
                if (ui_last)
                {
                    ui_last.SetActiv(false);
                    userInterfaceList.Remove(obj_last);
                }
                GameObject obj = userInterfaceList[userInterfaceList.Count - 1];
                if (obj)
                {
                    puppet ui = obj.GetComponent<puppet>();
                    if (ui)
                    {
                        ui.SetActiv(true);
                    }
                }
            }
        }
    }
    public bool IsClient()
    {
        return isClient;
    }
    public int GetClientContID()
    {
        if (isClient)
            return game.GetComponent<client>().ingameContID;
        return -1;
    }
}
