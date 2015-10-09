using UnityEngine;
using System.Collections;

public class DerpBoss : MonoBehaviour {

    public GameObject startPossition;
    public GameObject[] movePositions;
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
