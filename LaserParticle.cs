using UnityEngine;
using System.Collections;

public class LaserParticle : MonoBehaviour {

    public ParticleSystem particle;
	// Use this for initialization
	void Start () 
    {
       // particleSystem = GetComponent<ParticleSystem>();

        particle = GetComponent<ParticleSystem>();
        Destroy(this.gameObject, particle.duration); 
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    
}
