using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class DerpBoss : MonoBehaviour {

    

    public GameObject startPossition;
    public GameObject[] movePositions1;
    public GameObject[] movePositions2;
    public GameObject[] movePositions3;
    public GameObject[] movePositions4;

    public Vector3 plot1;
    public Vector3 plot2;
    public Vector3 plot3;
    public Vector3 plot4;

    public List<Vector3> course; 

    public GameObject Weapon;
    private int weaponSelection = 0;
    private int ammo = 1; 
    private bool fellDown;
    private bool flying = false; 
    private bool dropAmmo = false;
    private int health = 1000;
    private NetworkView nView; 
	// Use this for initialization
	void Start ()
    {
        nView = GetComponent<NetworkView>();

        startPossition = GameObject.Find("StartPoint");
        transform.position = new Vector3(startPossition.transform.position.x, startPossition.transform.position.y, startPossition.transform.position.z);
        movePositions1 = GameObject.FindGameObjectsWithTag("flypoint");
        movePositions2 = GameObject.FindGameObjectsWithTag("flypoint2");
        movePositions3 = GameObject.FindGameObjectsWithTag("flypoint3");
        movePositions4 = GameObject.FindGameObjectsWithTag("flypoint4"); 

        StartCoroutine(SpawnAmmo());
        StartCoroutine(SpawnAmmo()); 
	}
	
	// Update is called once per frame
	void Update ()
    {



        if (flying)
        {
            Flying(); 
        }
	}
    void PlotCourse()
    {
        int index1;
        int index2;
        int index3;
        int index4;

        index1 = Random.Range(0, movePositions1.Length);
        index2 = Random.Range(0, movePositions2.Length);
        index3 = Random.Range(0, movePositions3.Length);
        index4 = Random.Range(0, movePositions4.Length);

        plot1 = new Vector3(movePositions1[index1].transform.position.x, movePositions1[index1].transform.position.y, movePositions1[index1].transform.position.z);
        plot2 = new Vector3(movePositions2[index2].transform.position.x, movePositions2[index2].transform.position.y, movePositions2[index2].transform.position.z);
        plot3 = new Vector3(movePositions3[index3].transform.position.x, movePositions3[index3].transform.position.y, movePositions3[index3].transform.position.z);
        plot4 = new Vector3(movePositions4[index4].transform.position.x, movePositions4[index4].transform.position.y, movePositions4[index4].transform.position.z);

        course.Add(plot1);
        course.Add(plot2);
        course.Add(plot3);
        course.Add(plot4); 
    }
    private void Flying()
    {

    }
        
    IEnumerator FireAmmo()
    {
        flying = true; 
        yield return new WaitForSeconds(20);
        flying = false;

        StartCoroutine(SpawnAmmo());
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
