using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Sentry : MonoBehaviour {

    RaycastHit hit;
    float range = 1000.0f;
    LineRenderer line;
    public Material lineMaterial;
    public GameObject laserHit;
    public GameObject orb;
    bool ammo; 
    GameObject[] players;
    int playerChoice;
    bool fire = true;
    private Quaternion lookRotation;
    private float rotationSpeed;
    Vector3 direction;
    Transform firePoint;
    NetworkView nView;
    NetworkView pView;
    public GameObject death;
    bool alive;
    float serverHealth;
    public GameObject healthOrb1;
    public GameObject healthOrb2; 
    #region

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    private Quaternion roatation = Quaternion.identity;
    private Quaternion qSrot = Quaternion.identity;
    private Quaternion qErot = Quaternion.identity;

    #endregion

    int weapon = 0;
    int ammoCount; 
    public float health;
    public GUISkin skin;

    List<GameObject> movePositions = new List<GameObject>();
    int movePos = 0;
    Vector3 currentMovePosition;

    bool spawnedHealth; 
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

        movePositions.Add(GameObject.Find("StartBossPosition"));
        movePositions.Add(GameObject.Find("BossPosition2"));
        movePositions.Add(GameObject.Find("BossPosition3"));
        movePositions.Add(GameObject.Find("BossPosition4"));

        currentMovePosition = new Vector3(movePositions[movePos].transform.position.x, movePositions[movePos].transform.position.y, movePositions[movePos].transform.position.z);

        transform.position = currentMovePosition;

        spawnedHealth = false; 

        alive = true; 
        health = 1000f; 
        nView = GetComponent<NetworkView>();
        ammoCount = 50; 
        line = GetComponent<LineRenderer>();
        line.SetVertexCount(2);
        line.material = lineMaterial;
        line.SetColors(Color.red, Color.red);
        line.SetWidth(0.1f, .25f);

        firePoint = transform.FindChild("FirePoint"); 
        players = GameObject.FindGameObjectsWithTag("Player");
        playerChoice = 0;
        StartCoroutine(PlayerChoice());
        StartCoroutine(MissleRespawn());
        StartCoroutine(ChangePositions());
    }
	
	// Update is called once per frame
	void Update ()
    {
               

        if (nView.isMine)
        {
            BossHealth();
            MainAI(); 
        }
        else
        {
            SyncedMovement(); 
        }

        HealthRespawn(); 
        
    }
    
    void MainAI()
    {
        
        direction = ((players[playerChoice].transform.position - new Vector3(0,0,0))- transform.position).normalized;
        lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);

        if (fire)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            Vector3 pos = transform.position;
            Vector3 dir = transform.forward;
            if (Physics.Raycast(ray, out hit, range))
            {
                if (weapon == 0)
                {
                    line.enabled = true;
                    line.SetPosition(0, transform.position);
                    line.SetPosition(1, hit.point + hit.normal);
                    nView.RPC("Shot", RPCMode.All, pos, dir, fire);
                    nView.RPC("LaserHit", RPCMode.All, new Vector3(hit.point.x, hit.point.y, hit.point.z));
                }
                else
                {
                    line.enabled = false;
                }
                if (hit.collider.tag == "Player")
                {

                    switch (weapon)
                    {
                        case 0:
                            {
                                line.enabled = true;
                                line.SetPosition(0, transform.position);
                                line.SetPosition(1, hit.point + hit.normal);
                                nView.RPC("Shot", RPCMode.All, pos, dir, fire);
                                nView.RPC("LaserHit", RPCMode.All, new Vector3(hit.point.x, hit.point.y, hit.point.z));

                                pView = hit.collider.GetComponent<NetworkView>();

                                if (pView.isMine)
                                {
                                    PlayerMovement pMovement;
                                    pMovement = hit.collider.GetComponent<PlayerMovement>();
                                    pMovement.ClientTakeDamageFromWeapon(1);
                                }
                                else
                                {
                                    pView.RPC("NetworkTakeDamageFromWeapon", pView.owner, 1);
                                }

                                break;
                            }




                        case 1:
                            {
                                if (ammoCount > 0)
                                {
                                    Network.Instantiate(orb, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, 0);
                                    ammoCount--;
                                }
                                break;
                            }
                    }


                }

                #region lower level health
                /*
                if (health < 500)
                {
                    List<GameObject> currentPlayers = new List<GameObject>();   
                    foreach (GameObject gO in players)
                    {
                        currentPlayers.Add(gO); 
                    }

                    for (int i = 0; i < currentPlayers.Count; i++)
                    {

                    }
                }
                 * */
                #endregion

            }
            else
            {
                line.enabled = false;
            }
        }
        else
        {
            line.enabled = false;
        }
    }

    void OnGUI()
    {
        GUI.skin = skin;
        serverHealth = health;
        GUI.Box(new Rect((Screen.width / 2) - ((health / 2) / 2), 0, (health / 2), 50), "Mad Crystal", skin.GetStyle("BossHealth"));
        
    }
    void BossHealth()
    {
        if (health <= 0)
        {
            alive = false; 
            // Do an awesome explosion
        }
        else
        {
            alive = true;
            currentMovePosition = new Vector3(movePositions[movePos].transform.position.x, movePositions[movePos].transform.position.y, movePositions[movePos].transform.position.z);
            transform.position = Vector3.Lerp(transform.position, currentMovePosition, 1 * Time.deltaTime);
        }
        
        if (health <= 250)
        {
            if (!spawnedHealth)
            {
                Network.Instantiate(healthOrb1, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, 0);
                Network.Instantiate(healthOrb2, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, 0);
                spawnedHealth = true; 
            }
        }
                
    }
    void HealthRespawn()
    {
        if (nView.isMine)
        {
            if (GameObject.FindGameObjectWithTag("SentryHealth1"))
            {

                health = health + .5f;
                Debug.Log("Sentry One Found");
            }
            if (GameObject.FindGameObjectWithTag("SentryHealth2"))
            {
                health = health + .5f;
                Debug.Log("Sentry Two Found");
            }
            if (health >= 1000)
            {
                health = 1000; 
            }
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("SentryHealth1"))
            {

                health = health + .5f;
                Debug.Log("Sentry One Found");
            }
            if (GameObject.FindGameObjectWithTag("SentryHealth2"))
            {
                health = health + .5f;
                Debug.Log("Sentry Two Found");
            }
            if (health >= 1000)
            {
                health = 1000;
            }
        }

    }
    void CheckIfDead()
    {
        if (alive)
        {

        }
        else
        {
            Network.Instantiate(death, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, 0);
            Network.Destroy(this.gameObject); 
        }
    }
    IEnumerator PlayerChoice()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        yield return new WaitForSeconds(Random.Range(5, 15));
        playerChoice = Random.Range(0, players.Length);
        

        if (health >= 500)
        {

            weapon++;
            if (weapon > 1)
            {
                weapon = 0; 
            }
        }
        else
        {
            int chanceOfweaponChange = Random.Range(0, 100);
            if (chanceOfweaponChange > 50)
            {

                weapon = 1;
            }
            else
            {
                weapon = 0; 
            }
        }
        
        
        StartCoroutine(PlayerChoice());
        
    }
    IEnumerator ChangePositions()
    {
        yield return new WaitForSeconds(20);
        if (health < 250)
        {
            movePos++;

            if (movePos > movePositions.Count)
            {
                movePos = 0;
            }
        }


        StartCoroutine(ChangePositions());
    
    }
    IEnumerator MissleRespawn()
    {
        yield return new WaitForSeconds(.5f);
        ammoCount++;
        if (ammoCount > 1)
        {
            ammoCount = 1; 
        }
        StartCoroutine(MissleRespawn()); 
    }

    public void ClientTakeDamageFromWeapon(int dam)
    {
        health = health - (dam / 2);
        nView.RPC("SyncHealth", RPCMode.All , health); 
        CheckIfDead();
    }
    #region Network Junk
    [RPC]
    void LaserHit(Vector3 pos)
    {
        Instantiate(laserHit, pos, Quaternion.Euler(transform.rotation.x, -1 * transform.rotation.y, transform.rotation.z));
    }
    [RPC]
    void Shot(Vector3 position, Vector3 direction, bool show, NetworkMessageInfo info)
    {
        Ray ray = new Ray(position, direction);

        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hit.point + hit.normal);

        if (Physics.Raycast(ray, out hit, range))
        {
            if (show)
            {
                //Network.Instantiate(laserSound, position, Quaternion.identity, 0); 
                float audioVolumeX;
                float audioVolumeY;
                float audioVolumeZ;

                float audioVolume = 0;

                audioVolumeX = this.transform.position.x - info.networkView.GetComponent<Transform>().position.x;
                audioVolumeY = this.transform.position.y - info.networkView.GetComponent<Transform>().position.y;
                audioVolumeZ = this.transform.position.z - info.networkView.GetComponent<Transform>().position.z;

                audioVolume = Mathf.Sqrt((audioVolumeX - this.transform.position.x) * 2 + (audioVolumeY - this.transform.position.y * 2) + (audioVolumeZ - this.transform.position.z) * 2);
               // Debug.Log(audioVolume);

                if (audioVolume <= 100)
                {
                   // laserAudio.volume = 0;
                    if (audioVolume <= 80)
                    {
                      //  laserAudio.volume = 50;
                        if (audioVolume <= 50)
                        {
                            //laserAudio.volume = 100;
                        }

                    }

                }
                else
                {
                  //  laserAudio.volume = 0;
                }



               // laserAudio.Play();



                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, hit.point + hit.normal);
            }
            else
            {
                line.enabled = false;
            }
        }

    }

    [RPC]
    void NetworkTakeDamageFromWeapon(int damage)
    {
        health = health - (damage / 2);
        nView.RPC("SyncHealth", RPCMode.All, health);
        CheckIfDead(); 
    }
    [RPC]
    void SyncHealth(float cHealth)
    {
        health = cHealth;
        Sentry mSentry;
        mSentry = GameObject.FindGameObjectWithTag("Boss").GetComponent<Sentry>();
        mSentry.health = cHealth; 
    }
    #region NetworkSyncing
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        serverHealth = health; 
        Vector3 syncPosition = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        if (stream.isWriting)
        {

            syncPosition = transform.position;
            rot = transform.rotation;
            serverHealth = health; 

            stream.Serialize(ref syncPosition);

            stream.Serialize(ref rot);
            stream.Serialize(ref serverHealth);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref rot);
            stream.Serialize(ref serverHealth); 
            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncStartPosition = transform.position;
            syncEndPosition = syncPosition;

            qSrot = transform.rotation;
            qErot = rot;
        }


    }

    private void SyncedMovement()
    {
        syncTime += Time.deltaTime;
        transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);

        transform.rotation = Quaternion.Lerp(qSrot, qErot, syncTime / syncDelay);

    }
    #endregion

    #endregion
}