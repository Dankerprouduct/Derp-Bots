using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class NetworkManager : MonoBehaviour {


    float scroll = 0f; 
    public Material playerMat;
    public Color color;
    public Vector3 vColor; 
    public GUISkin skin; 
    bool showStartGame = false; 
    bool showWaitScreen = false; 
    NetworkView nView; 

    bool settings = false;
    bool serverLobbyGUI; 

    public string playeName = "Player";
    public List<string> playerDeaths = new List<string>();
    public List<string> playerNames = new List<string>();
    int connectedServer;
    
    public List<GameObject> spawnPoints;
    
    string serverName = " ";
    private const string typeName = "Derp Bots";
    private const string gameName = "Derp Bots";
    public GameObject playerPrefab;
    public GameObject spawnPoint;
    public GameObject sentry; 
    int mapNum = 0; 
    int serverPort;

    private HostData[] hostList;

    
   // PlayerMovement player; 
    
	// Use this for initialization
	void Start ()
    {
        StartCoroutine(RefreshRate()); 
        RefreshHostList();
        scroll = 0f; 
        nView = GetComponent<NetworkView>();
        
	}
    bool showMainMenu = true; 
	// Update is called once per frame

    void Awake()
    {
        playerDeaths.Clear(); 
        DontDestroyOnLoad(this.gameObject);
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject); 
        }
        if (Application.loadedLevel == 0)
        {
            showMainMenu = true;
            showStartGame = false;
            showWaitScreen = false;
            settings = false;
            serverLobbyGUI = false;
            playerNames.Clear(); 

        }
        
    }
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel(0);
            Network.Disconnect();
            Cursor.visible = true;
            showMainMenu = true;
            showStartGame = false;
            showWaitScreen = false;
            settings = false;
            serverLobbyGUI = false;
            playerNames.Clear(); 

        }
	}

    private void StartServer()
    {
        serverPort = Random.Range(2500, 3000); 
        Network.InitializeServer(10, serverPort, !Network.HavePublicAddress());
        Debug.Log(typeName +" "+ serverName); 
        MasterServer.RegisterHost(typeName, serverName);
    }

    void OnServerInitialized()
    {
        
        

       Debug.Log("Server Initializied");
       Debug.Log("Current Level: " + Application.loadedLevel.ToString());
       

    }

    IEnumerator WaitToJoin()
    {
        showWaitScreen = false; 
        Debug.Log("Got Spawns");
        yield return new WaitForSeconds(3);
        GameObject[] objects = GameObject.FindGameObjectsWithTag("spawn");

        foreach (GameObject gO in objects)
        {
            spawnPoints.Add(gO);
        }

        Debug.Log("Current Level: " + Application.loadedLevel.ToString());
        SpawnPlayer();
        
    }

    void OnGUI()
    {
        if (Application.loadedLevel == 0)
        {
            playerDeaths.Clear(); 
        }
        for (int i = 0; i < playerDeaths.Count; i++)
        {
            GUI.Label(new Rect(10, 13 * i, 1500, 50), playerDeaths[i]);
        }    
        
        
        if (showWaitScreen)
        {
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 250, 100), "WAITING FOR HOST TO START THE GAME..."); 
        }
        
        // !Network.isClient && !Network.isServer
        if (!Network.isClient && !Network.isServer)
        {
            
            
            if (showMainMenu)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                   // Application.Quit();
                }

                if (GUI.Button(new Rect(50, 50, 250, 100), "New Room"))
                {
                    showMainMenu = false;
                    serverLobbyGUI = true; 
                    
                }
                if (GUI.Button(new Rect(50, 150, 250, 100), "Refresh Hosts"))
                {
                    RefreshHostList();

                }
                if (GUI.Button(new Rect(50, 250, 250, 100), "Settings"))
                {
                    showMainMenu = false; 
                    settings = true; 

                }
                if (GUI.Button(new Rect(50, 350, 250, 100), "Escape"))
                {
                    Application.Quit();

                }

                if (hostList != null)
                {
                    for (int i = 0; i < hostList.Length; i++)
                    {
                        if (GUI.Button(new Rect(400, 50 + (110 * i), Screen.width / 4 + Screen.width / 4, 50), hostList[i].gameName))
                        {
                            JoinServer(hostList[i]);
                            showMainMenu = false; 
                            
                        }
                    }
                }
            }

            if (settings)
            {
                /// gui text for playername 
                playeName = GUI.TextField(new Rect(50, 30, 250, 20), playeName);

                if (GUI.Button(new Rect(50, 50, 250, 100), "Save Changes"))
                {
                    showMainMenu = true;
                    settings = false; 
                }
                color = Rgb(new Rect(10, 325, 200, 20), color);

                playerMat.color = color; 
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    showMainMenu = true;
                    settings = false; 
                }

            }

            
        }
        if (serverLobbyGUI) {
            if (!settings && !showMainMenu  && serverLobbyGUI)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    showMainMenu = true;
                    serverLobbyGUI = false; 
                }



                for (int i = 0; i < playerNames.Count; i++)
                {

                    GUI.Label(new Rect(50, 170 + (i * 25), 250, 500), playerNames[i]);
                }
                GUI.TextArea(new Rect(50, 170, 250, playerNames.Count * 25), "");

                scroll = Slider(scroll);

                serverName = GUI.TextField(new Rect(50, 30, 250, 20), serverName);

                if (GUI.Button(new Rect(400, 50 + scroll, 800, 50), "The Box"))
                {
                    mapNum = 1;
                }
                if (GUI.Button(new Rect(400, 100 + scroll, 800, 50), "The Box 2"))
                {
                    mapNum = 5;
                }
                if (GUI.Button(new Rect(400, 150 + scroll, 800, 50), "The Void"))
                {
                    mapNum = 2;
                }
                if (GUI.Button(new Rect(400, 200 + scroll, 800, 50), "Figure 8"))
                {
                    mapNum = 3;
                }
                if (GUI.Button(new Rect(400, 250 + scroll, 800, 50), "Sun Temple"))
                {
                    mapNum = 4;
                }
                if (GUI.Button(new Rect(400, 300 + scroll, 800, 50), "Hallway"))
                {
                    mapNum = 6;
                }
                if (GUI.Button(new Rect(400, 350 + scroll, 800, 50), "Ball Pen"))
                {
                    mapNum = 7;
                }
                if (GUI.Button(new Rect(400, 400 + scroll, 800, 50), "Tower"))
                {
                    mapNum = 8;
                }
                if (GUI.Button(new Rect(400, 450 + scroll, 800, 50), "Complex"))
                {
                    mapNum = 10;
                }
                if (GUI.Button(new Rect(400, 500 + scroll, 800, 50), "The Office"))
                {
                    mapNum = 11;
                }
                if (GUI.Button(new Rect(400, 550 + scroll, 800, 50), "Maya"))
                {
                    mapNum = 12; 
                }
                if (GUI.Button(new Rect(400, 600 + scroll, 800, 50), "The Maze"))
                {
                    mapNum = 13;
                }
                if (GUI.Button(new Rect(400, 650 + scroll, 800, 50), "Complex 2"))
                {
                    mapNum = 14; 
                }
                if (GUI.Button(new Rect(400, 700 + scroll, 800, 50), "Crystal Shard"))
                {
                    mapNum = 15;
                }
                if (Input.GetKeyDown(KeyCode.T))
                {
                    mapNum = 9;
                }
                if (!showStartGame)
                {
                    if (GUI.Button(new Rect(50, 50, 250, 100), "StartServer"))
                    {

                        if (mapNum != 0)
                        {
                            showStartGame = true;
                            StartServer();
                        }
                    }
                }
                if (showStartGame)
                {
                    if (GUI.Button(new Rect(50, 50, 250, 100), "Start Game") && showStartGame)
                    {
                        MasterServer.RegisterHost(typeName, serverName, mapNum.ToString());

                        nView.RPC("StartGame", RPCMode.All, mapNum);
                        //Application.LoadLevel(mapNum);

                        //StartCoroutine(WaitToJoin());
                        serverLobbyGUI = false;
                    }
                }
            }
        }
    }

    float Slider(float amount)
    {
        amount = CompundControls.VerticleLabelSlider(new Rect(300, 50, 50, 500), amount, 1000f, " ");
        return -amount;
    }

    private void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }

    private void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);

        
    }

    [RPC]
    void StartGame(int mapServer)
    {
        Application.LoadLevel(mapServer);
        Debug.Log("Rpc got map num " + mapServer);
        

        StartCoroutine(WaitToJoin());
    }



    void OnConnectedToServer()
    {
        Debug.Log("Server Joined");
       showWaitScreen = true;
       nView.RPC("ConnectedToLobby", RPCMode.All, playeName); 
    }

    [RPC]
    void ConnectedToLobby(string playName)
    {
        playerNames.Add(playName);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        print("Player is leaving. Destroying their objects");
        Network.DestroyPlayerObjects(player);
        Network.RemoveRPCs(player);

       // Application.LoadLevel(0); 

    }
    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Application.LoadLevel(0);
    }

    public void SpawnPlayer()
    {


        try
        {

            int index = Random.Range(0, spawnPoints.Count);

            Network.Instantiate(playerPrefab, new Vector3(spawnPoints[index].transform.position.x, spawnPoints[index].transform.position.y, spawnPoints[index].transform.position.z), Quaternion.Euler(270, 90, 0), 0);
            //player++; 
            
        }
        catch
        {
            SpawnPlayer(); 
        }
        if (Application.loadedLevel == 15)
        {
            Network.Instantiate(sentry, new Vector3(0, 20, 0), Quaternion.identity, 0);
        }

    }

    Color Rgb(Rect screenRect, Color rgb)
    {
      //  GUI.skin = skin;
        



            

            rgb.r = CompundControls.LabelSlider(screenRect, rgb.r, 1.0f, "Red");
            screenRect.y += 20;
            rgb.g = CompundControls.LabelSlider(screenRect, rgb.g, 1.0f, "Green");
            screenRect.y += 20;
            rgb.b = CompundControls.LabelSlider(screenRect, rgb.b, 1.0f, "Blue");

            vColor = new Vector3(rgb.r, rgb.g, rgb.b);
            
            // GUI.Box(new Rect(100, 450, 50, 50), "WASSUP", skin.GetStyle("ColorBox"));
            //skin.GetStyle("ColorBox").normal.textColor = rgb;

            return rgb;
    }


    [RPC]
    void RecentDeaths(string name, bool show)
    {

        Debug.Log(name + show + "Got to nmanager");
        // reset to null after duration of time
    }

    public void PlayerDied(string name)
    {
        if (playerDeaths.Count > 3)
        {
            playerDeaths.RemoveAt(0);
        }
        playerDeaths.Add(name);

    }
    IEnumerator RefreshRate()
    {
        yield return new WaitForSeconds(1);
        RefreshHostList();
        StartCoroutine(RefreshRate()); 
    }
        
}


