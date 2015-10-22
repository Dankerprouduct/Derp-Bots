using UnityEngine;
using System.Collections;

public class XenuBomb : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        Destroy(this.gameObject, GetComponent<ParticleSystem>().duration); 
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Explosion Boom" + other.name); 
    }
}
