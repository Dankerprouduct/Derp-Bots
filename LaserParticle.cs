using UnityEngine;
using System.Collections;

public class LaserParticle : MonoBehaviour {

    ParticleSystem particleSystem;
	// Use this for initialization
	void Start () 
    {
        particleSystem = GetComponent<ParticleSystem>();
        
        Destroy(this.gameObject, GetComponent<ParticleSystem>().duration); 
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    
}
