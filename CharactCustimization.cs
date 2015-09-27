using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class CharactCustimization : MonoBehaviour {

    public Material playerMat;
    public Color color;
    Vector3 vColor; 
	// Use this for initialization
	
	// Update is called once per frame

    bool showColor = false;

    NetworkView nView;
    NetworkViewID id;
    public GUISkin skin;

    int PlayerPrefab = 1;
    public List<GameObject> playerTypes;
    GameObject player;

    public string username = "";

    string recentDeath = "";

    NetworkManager nManager; 

    void Start()
    {
        player = GetComponent<GameObject>();

        nView = GetComponent<NetworkView>();
        sprite = this.GetComponent<MeshRenderer>();

        id = nView.viewID;
        nManager = GameObject.FindGameObjectWithTag("GM/NM").GetComponent<NetworkManager>(); 

        vColor = nManager.vColor;
        username = nManager.playeName; 
        nView.RPC("PlayerColor", RPCMode.All, vColor, username);

    }

    void CharacterChange()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
          //  SquarePlayer();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
          //  TrianglePlayer();
        }

    }

    private void SquarePlayer()
    {
        this.GetComponent<MeshFilter>().sharedMesh = playerTypes[0].GetComponent<MeshFilter>().sharedMesh; 
        
    }
    private void TrianglePlayer()
    {
        print("triangle player");
        this.GetComponent<MeshFilter>().sharedMesh = playerTypes[0].GetComponent<MeshFilter>().sharedMesh; 
    }

    void OnGUI()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            color = Rgb(new Rect(10, 325, 200, 20), color);

            

        }
    }
    Color Rgb(Rect screenRect, Color rgb)
    {
        GUI.skin = skin; 
        if (showColor)
        {



            username = GUI.TextField(new Rect(10, 305 - 60, 100, 20), username);

            rgb.r = CompundControls.LabelSlider(screenRect, rgb.r, 1.0f, "Red");
            screenRect.y += 20;
            rgb.g = CompundControls.LabelSlider(screenRect, rgb.g, 1.0f, "Green");
            screenRect.y += 20;
            rgb.b = CompundControls.LabelSlider(screenRect, rgb.b, 1.0f, "Blue");

            vColor = new Vector3(rgb.r, rgb.g, rgb.b);

           // GUI.Box(new Rect(100, 450, 50, 50), "WASSUP", skin.GetStyle("ColorBox"));
            skin.GetStyle("ColorBox").normal.textColor = rgb; 

            return rgb;
        }
        nView.RPC("PlayerColor", RPCMode.All, vColor, username);
        


         
        return rgb;
    }

    MeshRenderer sprite = new MeshRenderer();

    

    void Update()
    {
        playerMat.color = color;

      //  CharacterChange(); 
/*
        if (Input.GetKeyDown(KeyCode.C))
        {
            showColor = !showColor;
            Cursor.visible = showColor; 
        }
 * */
    }



    [RPC]
    void PlayerColor(Vector3 color,string userName, NetworkMessageInfo info)
    {

        MeshRenderer otherPlayerRenderer = info.networkView.GetComponent<MeshRenderer>(); 
            
        Material otherPlayerMaterial = info.networkView.GetComponent<Material>();
        Color nColor = new Color(color.x, color.y, color.z);

        otherPlayerRenderer.material.color = nColor;

        info.networkView.GetComponent<CharactCustimization>().username = userName; 
        
         
    }

    [RPC]
    void Died(NetworkMessageInfo info)
    {
        Debug.Log("died being called");
        recentDeath = info.networkView.GetComponent<CharactCustimization>().username; 
    }

}
