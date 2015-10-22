using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class DerpBoss : MonoBehaviour {

    

    public GameObject startPossition;
    public GameObject[] movePositions1;
    public GameObject[] movePositions2;
    public GameObject[] movePositions3;
    public GameObject[] movePositions4;
    public GameObject homePosition; 

    public Transform plot1;
    public Transform plot2;
    public Transform plot3;
    public Transform plot4;
    public Transform home; 

    
    private int weaponSelection = 0;
    private int ammo = 1; 
    private bool fellDown;
    private bool flying = false; 
    private bool dropAmmo = false;
    private int health = 1000;
    private NetworkView nView;

    public List<Transform> waypoints;

    private int currentIndex;
    private Transform currentWaypoint;

    bool interact = true; 
    float moveSpeed = 1f;
    float minDistance = 5f;

    bool preFlightCheck = false;
    public GameObject Bomb; 
    // Use this for initialization
    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }
	void Start ()
    {
        transform.rotation = Quaternion.Euler(0, 180, 180);  
        nView = GetComponent<NetworkView>();

        startPossition = GameObject.Find("StartPoint");
        transform.position = new Vector3(startPossition.transform.position.x, startPossition.transform.position.y, startPossition.transform.position.z);
        movePositions1 = GameObject.FindGameObjectsWithTag("flypoint");
        movePositions2 = GameObject.FindGameObjectsWithTag("flypoint2");
        movePositions3 = GameObject.FindGameObjectsWithTag("flypoint3");
        movePositions4 = GameObject.FindGameObjectsWithTag("flypoint4");
        homePosition = GameObject.FindGameObjectWithTag("Home");


        StartCoroutine(FireAmmo());
        StartCoroutine(SpawnAmmo());
        
        home = homePosition.transform;
        waypoints.Add(home);
        currentWaypoint = waypoints[0];
        currentIndex = 0;

	}
	
	// Update is called once per frame
	void Update ()
    {

        PlotCourse(); 

	}
    void PlotCourse()
    {
        if (!preFlightCheck)
        {
            waypoints.Clear(); 

            int index1;
            int index2;
            int index3;
            int index4;

            index1 = Random.Range(0, movePositions1.Length);
            index2 = Random.Range(0, movePositions2.Length);
            index3 = Random.Range(0, movePositions3.Length);
            index4 = Random.Range(0, movePositions4.Length);

            plot1 = movePositions1[index1].transform;
            plot2 = movePositions2[index2].transform;
            plot3 = movePositions2[index3].transform;
            plot4 = movePositions4[index4].transform;
            home = homePosition.transform;

            waypoints.Add(home);
            waypoints.Add(plot1);
            waypoints.Add(plot2);
            waypoints.Add(plot3);
            waypoints.Add(plot4);

            preFlightCheck = true;
            if (preFlightCheck)
            {
                Debug.Log("Finsihed PFC"); 
            }
        }
        else
        {
            Flying();
        }
    }


    private void Flying()
    {
        if (interact == true)
        {
            Debug.Log("Flying"); 
            MoveTowardWaypoint();

            if (Vector3.Distance(currentWaypoint.transform.position, transform.position) < minDistance)
            {
                currentIndex++;

                if (currentIndex > waypoints.Count - 1)
                {
                    currentIndex = 0;
                    preFlightCheck = false; 
                }

                currentWaypoint = waypoints[currentIndex];
            }
        }
    }
    void MoveTowardWaypoint()
    {
        Debug.Log("Moving to waypoint"+ currentWaypoint.name); 
        Vector3 direction = currentWaypoint.transform.position - transform.position;
        Transform facing = currentWaypoint.transform; 
        transform.LookAt(facing); 
        Vector3 moveVector = direction.normalized * moveSpeed; // *time.Delts
        
        transform.position += moveVector;
        
    }
    IEnumerator FireAmmo()
    {
        Debug.Log("Being CAleld"); 
        if (preFlightCheck)
        {
            Debug.Log("Fire");
            
            Network.Instantiate(Bomb, transform.position, Quaternion.identity, 0); 
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(FireAmmo());
    }
    IEnumerator SpawnAmmo()
    {
        yield return new WaitForSeconds(1);
        ammo++;
        if(ammo > 1)
        {
            ammo = 1; 
        }
        StartCoroutine(SpawnAmmo());
    }
}
