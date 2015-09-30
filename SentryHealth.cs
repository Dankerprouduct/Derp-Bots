using UnityEngine;
using System.Collections;

public class SentryHealth : MonoBehaviour {

    NetworkView nView;
    float health;
    Transform bossPosition;
    Transform startPosition;
    public int startPos;
    // Use this for initialization
    void Start()
    {
        nView.GetComponent<NetworkView>();
        health = 500;
        bossPosition = GameObject.FindGameObjectWithTag("Boss").GetComponent<Transform>();

        if (startPos == 1)
        {
            startPosition = GameObject.Find("SentryHealthPosition1").GetComponent<Transform>();
        }
        if (startPos == 2)
        {
            startPosition = GameObject.Find("SentryHealthPosition2").GetComponent<Transform>(); 
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (nView.isMine)
        {
            MainAi();
        }
	}

    void MainAi()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(startPosition.position.x, transform.position.y, startPosition.position.z), 2 * Time.deltaTime);
        transform.Rotate(Vector3.one * 3 * Time.deltaTime);


    }
    [RPC]
    void NetworkTakeDamageFromWeapon(int damage)
    {
        health = health - damage; 
    }
}
