using UnityEngine;
using System.Collections;
using System.Collections.Generic; 


[RequireComponent(typeof(LineRenderer))]

public class LaserBeam : MonoBehaviour
{

    List<string> insultStrings;

    string insult;

    bool outtaAmmo = false; 
    public AudioSource laserAudio; 
    
    Vector2 mouse;
    RaycastHit hit;
    float range = 1000.0f;
    LineRenderer line;
    public Material lineMaterial;

    bool showGui = true; 

    PlayerMovement player;
    Transform mPlayer;
    PlayerMovement playMovement;
    PlayerCamera playCam;
    Vehicle vehicle; 


    NetworkView nView;
    NetworkView effects;
    NetworkView ammoView;

    bool onVeh; 

    string text;
    

    int weapon = 0;
    int incendaryAmmo = 2;
    int laserAmmo = 100; 
    int flashAmmo = 3;
    int stasisAmmo = 5;
    int leviAmmo = 5;
    int seekerAmmo = 0;
    int NukeAmmo = 100; 

    bool menu; 
    public GUISkin skin; 

    public GameObject explosionPrefab;
    public GameObject FlashBang;
    public GameObject laserHit;
    public GameObject laserSound;
    public GameObject redFlare; 
    

    Random randInsults;
    int randInsuNum; 
    
    bool stasis = false; 
    void Start()
    {
        insultStrings = new List<string>()
        {
            "DERP",
            "DINGUS",
            "DUNDERHEAD",
            "DING-DONG",
            "NINCOMPOOP",
            "MORON",
            "IDIOT",
            "IDGIT",
            "STUPID",
            "DOODIE-HEAD",
            "DOOKIE-HEAD",
            "PISSBOI",
            "DUMMY, YOU",
            "NIT-WIT",
            "POO-STAIN",
            "MELLON-NOSE",
            "CHEESEBRAIN",
            "GENIUS",
            "GOPI"
        };
        
        mPlayer = transform.parent.transform.parent.GetComponent<Transform>();
        playMovement = transform.parent.transform.parent.GetComponent<PlayerMovement>();
        playCam = transform.parent.GetComponent<PlayerCamera>();

        effects = GetComponent<NetworkView>(); 

        line = GetComponent<LineRenderer>();
        line.SetVertexCount(2);
        line.material = lineMaterial;
        line.SetColors(Color.red, Color.red); 
        line.SetWidth(0.1f, .25f);

        StartCoroutine(IncendaryAmmoSpawn());
        StartCoroutine(LaserAmmoSpawn());
        StartCoroutine(FlashAmmoSpawn());
        StartCoroutine(Special());
        StartCoroutine(FireWorkGun()); 
    }
    
    void OnGUI()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            
            if (showGui)
            {
                GUI.skin = skin;
                /*
                GUI.Box(new Rect(Screen.width - 325, Screen.height - 115, 50, 50)," ", skin.GetStyle("IncendairyIcon"));
                GUI.Box(new Rect(Screen.width - 425, Screen.height - 60, laserAmmo * 2, 50), "Laser: " + laserAmmo.ToString(), skin.GetStyle("Laser"));
                GUI.Box(new Rect(0, Screen.height - 60, flashAmmo * 45, 50), "Flash" + flashAmmo.ToString(), skin.GetStyle("Flash"));
                GUI.Box(new Rect(0, Screen.height - 120, stasisAmmo * 45, 50), "Stasis" + stasisAmmo.ToString(), skin.GetStyle("Flash"));
                */

                GUI.Box(new Rect(10, Screen.height - 60, 50, 50), " ", skin.GetStyle("LaserIcon"));
                GUI.Box(new Rect(65, Screen.height - 60, 50, 50), " ", skin.GetStyle("IncendairyIcon"));
                GUI.Box(new Rect(120, Screen.height - 60, 50, 50), " ", skin.GetStyle("FlashIcon"));
                GUI.Box(new Rect(175, Screen.height - 60, 50, 50), " ", skin.GetStyle("StasisIcon"));
                GUI.Box(new Rect(230, Screen.height - 60, 50, 50), " ", skin.GetStyle("LeviIcon")); 

                GUI.Label(new Rect(25, Screen.height - 80, 25, 25), laserAmmo.ToString());
                GUI.Label(new Rect(87, Screen.height - 80, 25, 25), incendaryAmmo.ToString());
                GUI.Label(new Rect(144, Screen.height - 80, 25, 25), flashAmmo.ToString());
                GUI.Label(new Rect(197, Screen.height - 80, 25, 25), stasisAmmo.ToString());
                GUI.Label(new Rect(247, Screen.height - 80, 25, 25), leviAmmo.ToString());


                if (weapon == 0)
                {
                    GUI.Box(new Rect(10, Screen.height - 60, 50, 50), " ", skin.GetStyle("HighlightIcon"));
                }
                if (weapon == 1)
                {
                    GUI.Box(new Rect(65, Screen.height - 60, 50, 50), " ", skin.GetStyle("HighlightIcon"));
                }
                if (weapon == 2)
                {
                    GUI.Box(new Rect(120, Screen.height - 60, 50, 50), " ", skin.GetStyle("HighlightIcon"));
                }
                if (weapon == 3)
                {
                    GUI.Box(new Rect(175, Screen.height - 60, 50, 50), " ", skin.GetStyle("HighlightIcon"));
                }
                if (weapon == 4)
                {
                    GUI.Box(new Rect(230, Screen.height - 60, 50, 50), " ", skin.GetStyle("HighlightIcon"));
                }

                if (outtaAmmo)
                {
                    GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height / 2, 150, 50), "OUTTA AMMO YOU " + insult);
                }

            }
        }
    }

    void Update()
    {

        DevOptions(); 

        if (laserAmmo > 100)
        {
            laserAmmo = 100; 
        }
        if (incendaryAmmo > 5)
        {
            incendaryAmmo = 5;
        }
        if (flashAmmo > 3)
        {
            flashAmmo = 3; 
        }
        if (leviAmmo > 1)
        {
            leviAmmo = 1;
        }
        if (stasisAmmo > 5)
        {
            stasisAmmo = 5; 
        }


        if (transform.position.y < -100)
        {
            showGui = false;
        }
        else
        {
            showGui = true; 
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            menu = !menu; 
        }

        if (GetComponent<NetworkView>().isMine)
        {
            if (!menu)
            {
                DoStuff();
            }
        }

    }
    void DevOptions()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            incendaryAmmo++; 
        }
    }

    void DoStuff()
    {
        Ray ray = new Ray(transform.parent.GetComponent<Camera>().transform.position, transform.parent.GetComponent<Camera>().transform.forward);
        Vector3 pos = transform.parent.GetComponent<Camera>().transform.position;
        Vector3 dir = transform.parent.GetComponent<Camera>().transform.forward;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            weapon = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            weapon = 4; 
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            weapon++;
            if (weapon >= 4)
            {
                weapon = 4;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            weapon--;
            if (weapon <= 0)
            {
                weapon = 0;
            }
        }

        if (Physics.Raycast(ray, out hit, range))
        {



            #region
           /*
            if (weapon == 4)
            {
                
                if (Input.GetKeyDown(KeyCode.Space))
                {
                   // print("vehicle selected"); 
                    onVeh = !onVeh;
                    Debug.Log(onVeh);
                    if (hit.collider.tag == "Vehicle")
                    {
                        
                        //Debug.Log("Hit Vechicle");
                        //Debug.Log(onVeh); 
                        if (onVeh)
                        {
                            playCam.InVehicle();
                       //     playMovement.InVehicle();


                            mPlayer.parent = hit.collider.GetComponent<Transform>();
                            mPlayer.localPosition = new Vector3(-2.486229f, 0.32169f, 1.553621f);
                            mPlayer.transform.localRotation = Quaternion.Euler(0.2488809f, 5.57256f, 277.1208f);


                            vehicle = transform.parent.transform.parent.transform.parent.GetComponent<Vehicle>();
                            vehicle.onVehicle = !vehicle.onVehicle;

                            
                        }
                        
                        
                        
                        
                    }
                }
            }
            else
            {
                range = 1000.0f;
            }
            * */
#endregion 

           

                range = 1000.0f;
                

                if (Input.GetMouseButtonDown(0))
                {

                    line.enabled = true;
                    line.SetPosition(0, transform.position);
                    line.SetPosition(1, hit.point + hit.normal);

                    effects.RPC("Shot", RPCMode.All, pos, dir, true);
                    laserAudio.Play();

                    AmmoChecks();

                    if (hit.collider.tag == "PlayerDeathParticle")
                    {
                        hit.rigidbody.AddForceAtPosition(2000 * dir, hit.point);
                    }
                    
                    switch (weapon)
                    {

                        case 0:
                            {
                                // Regular Laser
                                // put if ammo > 0 or whatever
                                if (laserAmmo > 0)
                                {

                                    print("laser");

                                    if (hit.collider.tag == "Player")
                                    {
                                        nView = hit.collider.GetComponent<NetworkView>();
                                        nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 5); 
                                    }
                                if (hit.collider.tag == "Orb")
                                {
                                    nView = hit.collider.GetComponent<NetworkView>();
                                    nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 5);
                                    if (nView.isMine)
                                    {
                                        Companion companion;
                                        companion = nView.GetComponent<Companion>();
                                        companion.ClientTakeDamageFromWeapon(); 
                                    }
                                    else
                                    {
                                        nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 5);
                                    }
                                }

                                    //// BOSSS 


                                if (hit.collider.tag == "Boss")
                                {
                                    nView = hit.collider.GetComponent<NetworkView>();
                                    nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 5);
                                    if (nView.isMine)
                                    {
                                        Sentry boss;
                                        boss = nView.GetComponent<Sentry>();
                                        boss.ClientTakeDamageFromWeapon(10);
                                    }
                                    else
                                    {
                                        nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 5);
                                    }
                                }





                                if (laserAmmo == 0)
                                    {
                                        StartCoroutine(LaserAmmoSpawn());
                                    }

                                    Vector3 explosionPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                                    effects.RPC("LaserHit", RPCMode.All, explosionPos);

                                    laserAmmo--; 
                                }
                                

                                break;
                            }
                        case 1:
                            {
                                // Incendairy Laser
                                if (incendaryAmmo > 0)
                                {
                                    print("Incendairy");
                                    if (hit.collider.tag == "Player")
                                    {
                                        nView = hit.collider.GetComponent<NetworkView>();
                                        nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 10);
                                    }
                                    if (hit.collider.tag == "Orb")
                                    {
                                        nView = hit.collider.GetComponent<NetworkView>();
                                        nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 5);
                                        if (nView.isMine)
                                        {
                                            Companion companion;
                                            companion = nView.GetComponent<Companion>();
                                            companion.ClientTakeDamageFromWeapon(); 
                                        }
                                        else
                                        {
                                            nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 5);
                                        }
                                    }
                                    

                                    if (hit.collider.tag == "Boss")
                                     {
                                    nView = hit.collider.GetComponent<NetworkView>();
                                    nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 10);
                                    if (nView.isMine)
                                    {
                                        Sentry boss;
                                        boss = nView.GetComponent<Sentry>();
                                        boss.ClientTakeDamageFromWeapon(10);
                                    }
                                    else
                                    {
                                        nView.RPC("NetworkTakeDamageFromWeapon", nView.owner, 10);
                                    }
                                }
                                    incendaryAmmo--;
                                    Vector3 explosionPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                                    effects.RPC("Explosion", RPCMode.All, explosionPos);
                                }
                                else
                                {
                                    weapon = 0;
                                }
                                
                                break;
                            }
                        case 2:
                            {
                                // Flash Laser
                                if (flashAmmo > 0)
                                {

                                    print("flash"); 
                                    flashAmmo--;
                                    Vector3 explosionPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                                    effects.RPC("FlashExplosion", RPCMode.All, explosionPos);

                                }
                                else
                                {
                                    weapon = 0;
                                }
                                break;
                            }
                        case 3:
                            {
                                // Stasis Laser
                                if (stasisAmmo > 0)
                                {
                                    if (hit.collider.tag == "Player")
                                    {
                                        print("stasis");
                                        

                                        nView = hit.collider.GetComponent<NetworkView>();
                                        nView.RPC("Stasis", nView.owner);
                                    }
                                    stasisAmmo--;
                                }
                                else
                                {
                                    weapon = 0;
                                }
                                break; 
                            }
                        case 4:
                            {
                                // Levi

                                if (leviAmmo > 0)
                                {
                                    if (hit.collider.tag == "Player")
                                    {
                                        nView = hit.collider.GetComponent<NetworkView>();
                                        nView.RPC("Levitation", nView.owner, 5);
                                        
                                        

                                    }
                                    leviAmmo--; 
                                }
                                else
                                {
                                    weapon = 0;
                                }
                                break;
                            }
                        case 5:
                            {
                                // new ammo goes here, sniffer
                                if (seekerAmmo > 0)
                                {

                                    if (hit.collider.tag == "Player")
                                    {
                                        nView = hit.collider.GetComponent<NetworkView>();
                                        nView.RPC("Seek", nView.owner);
                                    }
                                    seekerAmmo--;
                                }
                                else
                                {
                                    weapon = 0;
                                }
                                

                                break;
                            }
                        case 6:
                            {
                                if (NukeAmmo > 0)
                                {
                                    Vector3 explosionPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                                    effects.RPC("NukeHit", RPCMode.All, explosionPos);
                                }
                                else
                                {
                                    weapon = 0;
                                }

                                break; 
                            }

                          
                    }
                }
                else
                {
                    effects.RPC("Shot", RPCMode.All, pos, dir, false);
                    line.enabled = false;
                }
                     



            

            
        }
    }

    void AmmoChecks()
    {
        randInsults = new Random();
        int randNum = Random.Range(0, insultStrings.Count);

        insult = insultStrings[randNum]; 


        if (weapon == 0)
        {
            if (laserAmmo <= 0)
            {
                outtaAmmo = true;
            }
            else
            {
                outtaAmmo = false;
            }
        }

        if (weapon == 1)
        {
            if (incendaryAmmo <= 0)
            {
                outtaAmmo = true;
            }
            else
            {
                outtaAmmo = false;
            }
        }
        if (weapon == 2)
        {
            if (flashAmmo <= 0)
            {
                outtaAmmo = true;
            }
            else
            {
                outtaAmmo = false;
            }
        }
        if (weapon == 3)
        {
            if (stasisAmmo <= 0)
            {
                outtaAmmo = true;
            }
            else
            {
                outtaAmmo = false;
            }
        }
        if (weapon == 4)
        {
            if (leviAmmo <= 0)
            {
                outtaAmmo = true;
            }
            else
            {
                outtaAmmo = false;
            }
        }
    }

    IEnumerator FireWorkGun()
    {
        yield return new WaitForSeconds(60);
        leviAmmo++;
        AmmoChecks();
        StartCoroutine(FireWorkGun());
    }

    IEnumerator IncendaryAmmoSpawn()
    {
      
        yield return new WaitForSeconds(5);
        incendaryAmmo++;
        AmmoChecks();
        StartCoroutine(IncendaryAmmoSpawn());
        
    }
    IEnumerator LaserAmmoSpawn()
    {
    
        yield return new WaitForSeconds(1);
        laserAmmo++;
        AmmoChecks(); 
        StartCoroutine(LaserAmmoSpawn());
        
    }
    IEnumerator FlashAmmoSpawn()
    {
     
        yield return new WaitForSeconds(3);
        flashAmmo++;
        AmmoChecks();
        StartCoroutine(FlashAmmoSpawn());

    }
    IEnumerator Special()
    {

        yield return new WaitForSeconds(10);
        stasisAmmo++;
        //leviAmmo++;
        AmmoChecks(); 
        StartCoroutine(Special());

    }
    [RPC]
    void NukeHit(Vector3 pos)
    {
        Instantiate(redFlare, pos, Quaternion.Euler(270f, 2.350006f, 0f));
    }

    [RPC]
    void Explosion(Vector3 pos)
    {
        Instantiate(explosionPrefab, pos, Quaternion.Euler(270f,2.350006f, 0f));
    }
    [RPC]
    void LaserHit(Vector3 pos)
    {
        Instantiate(laserHit, pos, Quaternion.Euler(transform.rotation.x, -1 * transform.rotation.y, transform.rotation.z));
    }
    [RPC]
    void FlashExplosion(Vector3 pos)
    {
        Instantiate(FlashBang, pos, Quaternion.Euler(270f, 2.350006f, 0f));
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
                Debug.Log(audioVolume); 

                if (audioVolume <= 100)
                {
                    laserAudio.volume = 0;
                    if (audioVolume <= 80)
                    {
                        laserAudio.volume = 50;
                        if (audioVolume <= 50)
                        {
                            laserAudio.volume = 100; 
                        }

                    }

                }
                else
                {
                    laserAudio.volume = 0;
                }
                
                
                
                laserAudio.Play();



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

}