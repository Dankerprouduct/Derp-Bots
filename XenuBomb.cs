using UnityEngine;
using System.Collections;

public class XenuBomb : MonoBehaviour {

    // Use this for initialization
    public GameObject explosion;
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
        if (other.tag == "Floor")
        {
            Debug.Log("Explosion Boom " + other.name);
            Network.Instantiate(explosion, transform.position, Quaternion.identity, 0); 
            Destroy(this.gameObject);
        }
    }
}
