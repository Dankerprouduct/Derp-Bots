using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    void OnGUI()
    {
        
        if (GUI.Button(new Rect(100, 200, 100, 200), "The Box"))
        {
            Application.LoadLevel(1); 
        }
        if (GUI.Button(new Rect(250, 200, 100, 200), "The Void"))
        {
            Application.LoadLevel(2); 
        }
        if (GUI.Button(new Rect(400, 200, 100, 200), "Figure 8"))
        {
            Application.LoadLevel(3);
        }

        if (GUI.Button(new Rect(550, 200, 100, 200), "Sun Temple"))
        {
            Application.LoadLevel(4);
        }

        if (GUI.Button(new Rect(700, 200, 100, 200), "The Box 2"))
        {
            Application.LoadLevel(5);
        }
        if (GUI.Button(new Rect(850, 200, 100, 200), "Hallway"))
        {
            Application.LoadLevel(6);
        }
        if (GUI.Button(new Rect(100, 450, 100, 200), "Ball Pen"))
        {
            Application.LoadLevel(7);
        }
        if (GUI.Button(new Rect(250, 450, 100, 200), "Tower"))
        {
            Application.LoadLevel(8);
        }
         

    }
}
