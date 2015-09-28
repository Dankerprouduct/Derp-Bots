using UnityEngine;
using System.Collections;

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
    float health; 
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
        health = 5000; 
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

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            weapon = 1; 
        }

        if (!nView.isMine)
        {
            SyncedMovement(); 
        }
        transform.position = new Vector3(0, 20, 0);
        direction = (players[playerChoice].transform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);

        if (fire)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            Vector3 pos = transform.position;
            Vector3 dir = transform.forward;
            if (Physics.Raycast(ray, out hit, range))
            {
                if(weapon == 0)
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

        GUI.Box(new Rect(0, Screen.width / 2, (health / 2), 50), "Mad Crystal");
    }
    void BossHealth()
    {
        if (health <= 0)
        {
            // Do an awesome explosion
        }
    }
    IEnumerator PlayerChoice()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        yield return new WaitForSeconds(Random.Range(5, 15));
        playerChoice = Random.Range(0, players.Length);
        

        if (health >= 2500)
        {
            weapon = 0;
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
        ammoCount = 50; 
        
        StartCoroutine(PlayerChoice());
        //fire = !fire; 
    }
    [RPC]
    void NetworkTakeDamageFromWeapon(int dam)
    {

    }
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
        health = health - damage; 
    }
    #region NetworkSyncing
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        if (stream.isWriting)
        {

            syncPosition = transform.position;
            rot = transform.rotation;


            stream.Serialize(ref syncPosition);

            stream.Serialize(ref rot);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref rot);

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
}
