using UnityEngine;
using System.Collections;

public class SpawnNuke : MonoBehaviour {

    Transform nuke;
    public Gamemode nukeExplosion;
	// Use this for initialization
	void Start ()
    {
        nuke = transform.GetChild(0).transform;
       // Destroy(this.gameObject, 30); 
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (nuke.transform.position.z < 5)
        {
          //  Instantiate(nukeExplosion); 
            Destroy(this.gameObject, 1); 
        }
	}
}
