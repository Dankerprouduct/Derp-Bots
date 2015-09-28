using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    RaycastHit rayCast;
    Transform firePoint; 
    public Transform lazerTrailPrefab; 
	// Use this for initialization
	void Start ()
    {

        firePoint = transform.FindChild("FirePoint"); 
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GetComponent<NetworkView>().isMine)
        {
            Cursor.visible = true;
            Fire();
        }
        else
        {
            Cursor.visible = false; 
        }
	}


    void Fire()
    {

        if (Input.GetMouseButtonDown(0))
        {
          //  Debug.Log("Fire Shot");
            // FirePoint
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(firePoint.position , fwd, out hit))
            {

              //  Debug.Log(hit.collider.name.ToString());

              //  Debug.DrawLine(firePoint.position, hit.point, Color.green); 
            }
            Network.Instantiate(lazerTrailPrefab, firePoint.position, firePoint.rotation, 0); 
            
        }
    }
}
