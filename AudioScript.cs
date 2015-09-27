using UnityEngine;
using System.Collections;

public class AudioScript : MonoBehaviour {

    float playerDistance; 
    
    AudioSource source;
	// Use this for initialization
	void Start () {
        //playerDistance = transform.position - 
        source = GetComponent<AudioSource>();
        //source.volume = 
        Destroy(this.gameObject, 5); 
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
