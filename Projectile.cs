using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	// Use this for initialization
    Rigidbody rigidBody;
	void Start () {
        rigidBody = GetComponent<Rigidbody>();

       // rigidBody.AddForce(Vector3.right * 10000); 
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.Translate(Vector3.right * 10);    

        

        Destroy(this.gameObject, 3); 
	}
}
