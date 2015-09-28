using UnityEngine;
using System.Collections;

public class MoveEnviroment : MonoBehaviour {

    // Use this for initialization

    bool interact = true;

    NetworkView nView; 
    public bool kill; 

    public Transform[] waypoints;

    private int currentIndex;
    private Transform currentWaypoint;

    public float moveSpeed;
    float minDistance = .01f;

    void Start()
    {
        
        currentWaypoint = waypoints[0];
        currentIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (interact == true)
        {
            MoveTowardWaypoint();

            if (Vector3.Distance(currentWaypoint.transform.position, transform.position) < minDistance)
            {
                currentIndex++;

                if (currentIndex > waypoints.Length - 1)
                {
                    currentIndex = 0;

                }

                currentWaypoint = waypoints[currentIndex];
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (kill)
        {
            if (other.tag == "Player")
            {
                nView = other.gameObject.GetComponent<NetworkView>();
                nView.RPC("TakeDamage", nView.owner, 1);
            }
        }
    }
    void MoveTowardWaypoint()
    {
        Vector3 direction = currentWaypoint.transform.position - transform.position;
        Vector3 moveVector = direction.normalized * moveSpeed; // *time.Delts
        transform.position += moveVector;

    }
}
