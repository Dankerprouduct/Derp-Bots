using UnityEngine;
using System.Collections;

public class DestroyTime : MonoBehaviour {

    public float timer = 2f;
    NetworkView nView;
	// Use this for initialization
	void Start()
    {
       // nView = GetComponent<NetworkView>();
        Destroy(this.gameObject, timer); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            nView = other.GetComponent<NetworkView>();
            nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 50);
        }
    }
}
