using UnityEngine;
using System.Collections;

public class IncendaryAmmo : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    [RPC]
    void AddIncenAmmo()
    {
        this.GetComponent<MeshRenderer>().enabled = false; 
    }
}
