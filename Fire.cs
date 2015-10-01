using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {


    NetworkView nView;
	// Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider coll)
    {
        if (coll.tag == "Player")
        {
           // GameObject hitPlayer = coll.gameObject;

            nView = coll.GetComponent<NetworkView>();

            nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 1); 

        }
        if (coll.tag == "Boss")
        {
            // GameObject hitPlayer = coll.gameObject;
            nView = coll.GetComponent<NetworkView>(); 
            nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 1); 

            if (nView.isMine)
            {
                Sentry companion;
                companion = nView.GetComponent<Sentry>();
                companion.ClientTakeDamageFromWeapon(1);
            }
            else
            {
                nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 1);
            }
        }
    }
}
