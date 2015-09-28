using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

    private Quaternion lookRotation;
    private float rotationSpeed;
    Vector3 direction;
    // Use this for initialization
    void Start ()
    {
        GameObject.Destroy(this.gameObject, 8);
	}
	
	// Update is called once per frame
	void Update ()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");

        direction = (target.transform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1);

        transform.Translate(Vector3.forward * 20 * Time.deltaTime);
    }
}
