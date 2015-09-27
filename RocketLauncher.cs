using UnityEngine;
using System.Collections;

public class RocketLauncher : MonoBehaviour {

    public GameObject propellent; 
    public Transform startPosition;
    GameObject[] players;
    int playerChoice = 0;

    private Quaternion lookRotation;
    private float rotationSpeed;
    Vector3 direction;
    Transform firePoint;
    int firepointChoice = 2; 
    bool fire; 
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
        firePoint = transform.GetChild(0); 
        players = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(RocketAI());
    }
	
	// Update is called once per frame
	void Update ()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        transform.position = new Vector3(startPosition.position.x, transform.position.y, transform.position.z);

        direction = (players[playerChoice].transform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);

        if (fire)
        {
            Network.Instantiate(propellent, transform.GetChild(firepointChoice).position, Quaternion.Euler(transform.GetChild(firepointChoice).rotation.x, transform.GetChild(firepointChoice).rotation.y, transform.GetChild(firepointChoice).rotation.z), 0); 
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            fire = true;
        }
        else
        {
            fire = false; 
        }
    }

    IEnumerator RocketAI()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        firepointChoice = Mathf.RoundToInt(Random.Range(2,4));


        yield return new WaitForSeconds(5);

        StartCoroutine(RocketAI());
    }
}
