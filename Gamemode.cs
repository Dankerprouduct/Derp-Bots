using UnityEngine;
using System.Collections;

public class Gamemode : MonoBehaviour {

    NetworkView nview; 
    int gameMode = 0; 
	// Use this for initialization
	void Start () 
    {
        nview = GetComponent<NetworkView>(); 
        LoadGameType(); 
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void LoadGameType()
    {
        if (gameMode == 0)
        {
            // Vanilla
            nview.RPC("GameType", RPCMode.All, 0);
        }
        if (gameMode == 1)
        {
            // OneShot One Kill
            nview.RPC("GameType", RPCMode.All, 1); 
        }
        if (gameMode == 2)
        {
            // Team Deathmath
            nview.RPC("GameType", RPCMode.All, 2);
        }
        if (gameMode == 4)
        {
            // King Of the hill
        }
    }

    [RPC]
    void GameType(int gType)
    {   

    }

}
