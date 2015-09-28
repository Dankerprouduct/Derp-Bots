using UnityEngine;
using System.Collections;

public class RotateEnviroment : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    public float speed; 
	// Update is called once per frame
	void Update () 
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime); 
    }
}
