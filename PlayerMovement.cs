using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 

public class PlayerMovement : MonoBehaviour
{
    #region bools 
    bool levitation;
    bool stasis;
    bool alive; 
    bool showSense;    
    #endregion;

    #region quantitative data
    float health;
    public float speed;
    private string playerName;
    public string nPName; 
    public List<string> playerDeaths = new List<string>();
    public List<GameObject> playerListObject;
    GameObject[] playerObjects;
    List<string> playerKillNames = new List<string>();
    List<int> playerKillScore = new List<int>();
    public int kills = 0; 
    #endregion

    #region player physical properties
    public GUISkin skin;
    Rigidbody rigidBody;
    BoxCollider playerCollider;
    MeshRenderer playerRenderer;
    NetworkView nView;
    NetworkManager nManager; 
    public GameObject popExplosion;
    public GameObject explosionPeices;
    ParticleSystem smokingEffect;
    NetworkView playerRecieved; 

    #endregion
       
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

    #region Camera Controls

    PlayerCamera playCamScript;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 5f;

    public float minimumX = -360F;
    public float maximumX = 360F;

    float rotationX = 0F;
    Quaternion originalRotation;
    #endregion
    void Start()
    {
        smokingEffect = GetComponent<ParticleSystem>();
        smokingEffect.Stop(); 
        Cursor.visible = false; 
        nView = GetComponent<NetworkView>();
        nManager = GameObject.FindGameObjectWithTag("GM/NM").GetComponent<NetworkManager>();
        
        playerName = nManager.playeName;
        nPName = playerName; 
        // Resets health        

        health = 100; 
        rigidBody = GetComponent<Rigidbody>();
        alive = true; 
        CheckStats();

        if (rigidBody)
            rigidBody.freezeRotation = true;
        originalRotation = transform.localRotation;

        
        

    }
    void Update()
    {
        if (nView.isMine)
        {
            if (!showSense)
            {
                Movement();
                Cam();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                showSense = !showSense;
                Cursor.visible = showSense;
            }
            if (transform.position.y < -100)
            {
                health = 0;
                CheckStats();
                
            }
            DevControls();
        }
        else
        {
            SyncedMovement();
        }
    }
    void DevControls()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ClientTakeDamageFromWeapon(health); 
        }
    }
    void Movement()
    {
        
        if (alive)
        {
            if (!stasis)
            {
                // Forward
                if (Input.GetKey(KeyCode.W))
                {
                    rigidBody.position += (-1 * this.transform.right) * speed * Time.deltaTime;
                }
                // Backward
                if (Input.GetKey(KeyCode.S))
                {
                    rigidBody.position += (this.transform.right) * speed * Time.deltaTime;
                }
                // Left
                if (Input.GetKey(KeyCode.A))
                {
                    rigidBody.position += this.transform.up * speed * Time.deltaTime;
                }
                // Right
                if (Input.GetKey(KeyCode.D))
                {
                    rigidBody.position += -1 * this.transform.up * speed * Time.deltaTime;
                }

                if (levitation)
                {
                    rigidBody.position += this.transform.forward * speed * Time.deltaTime;
                }
            }
        }
        

    }

    void OnGUI()
    {

        for (int i = 0; i < playerDeaths.Count; i++)
        {
            GUI.Label(new Rect(10, 10 * i, 1500, 50), playerDeaths[i] + "  was decimated");
        }
        if (nView.isMine)
        {
            if (showSense)
            {
                MouseXSensitivity(); 
            }
            if (alive)
            {
                GUI.skin = skin;

                GUI.Box(new Rect(Screen.width - 225, Screen.height - 60, health * 2, 50), "Health", skin.GetStyle("box"));
                
            }

            ShowKills(); 
            
        }

    }

    void ShowKills()
    {
        GUI.skin = skin;
        float width = 350;
        float height = 200; 
        if (Input.GetKey(KeyCode.Tab))
        {
            playerKillNames.Clear(); 
            playerKillScore.Clear();
            playerListObject.Clear(); 
            playerObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject gObject in playerObjects)
            {
                playerListObject.Add(gObject);

                PlayerMovement pMovement;
                pMovement = gObject.GetComponent<PlayerMovement>();
                playerKillNames.Add(pMovement.PlayerName()); 
                playerKillScore.Add(pMovement.PlayerScore());
            }

            for (int x = 0; x < playerListObject.Count; x++)
            {
                PlayerMovement pMovement;
                pMovement = playerListObject[x].GetComponent<PlayerMovement>();

                playerKillNames.Add(pMovement.PlayerName());
                playerKillScore.Add(pMovement.PlayerScore());
            }

                GUI.Box(new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height), "Score", skin.GetStyle("textfield"));

            for (int i = 0; i < playerKillNames.Count; i++)
            {
                GUI.Label(new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2 - 25)+ i * 25, width, height), playerKillNames[i].ToString() + "| Kills " + playerKillScore[i].ToString());
            }
        }
    }
    public string PlayerName()
    {
        return playerName; 
    }
    public int PlayerScore()
    {
        return kills; 
    }
    public void AddKill()
    {
        kills++; 
    }
    void MouseXSensitivity()
    {
        sensitivityX = CompundControls.LabelSlider(new Rect(10, 380, 200, 20), sensitivityX, 5f, "X Sensitivity");
    }
    public void ClientTakeDamageFromWeapon(float damage)
    {
        health = health - damage;
        CheckStats(); 
    }

    void CheckStats()
    {
        if (alive)
        {
            if (health > 0)
            {
                playerCollider = GetComponent<BoxCollider>();
                playerRenderer = GetComponent<MeshRenderer>();

                alive = true;
                playerCollider.enabled = true;
                playerRenderer.enabled = true;
                transform.GetChild(0).GetComponent<NetworkView>().enabled = true;
                transform.GetChild(0).GetComponent<PlayerCamera>().enableCam = true;
            }
            else
            {
                alive = false; 
                playerCollider.enabled = false;
                playerCollider.enabled = false;
                nView.RPC("MeshHide", RPCMode.All, false);
                transform.GetChild(0).GetComponent<NetworkView>().enabled = false;
                transform.GetChild(0).GetComponent<PlayerCamera>().enableCam = false;
                nView.RPC("PlayerDied", RPCMode.AllBuffered, playerName);
                
                PlayerMovement sendDeath;
               // sendDeath = playerRecieved.GetComponent<PlayerMovement>();
              //  sendDeath.AddKill(); 
                
                Network.Instantiate(explosionPeices, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, 0);  
                StartCoroutine(RespawnTime()); 
            }

            if (health <= 25)
            {
                nView.RPC("SmokingEffect",RPCMode.All, true);
                smokingEffect.Play();
            }
            else
            {
                nView.RPC("SmokingEffect",RPCMode.All, false); 
                smokingEffect.Stop();
            }

        }
    }

    #region Camera Movement 
    void Cam()
    {
        if (showSense == false)
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                //   rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                //   rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.forward);
                //  Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

                transform.localRotation = originalRotation * xQuaternion;
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.forward);
                transform.localRotation = originalRotation * xQuaternion;
            }
        }
        //
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
    #endregion

    IEnumerator RespawnTime()
    {

        yield return new WaitForSeconds(3);

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);

        transform.position = new Vector3(spawnPoints[spawnIndex].transform.position.x, spawnPoints[spawnIndex].transform.position.y, spawnPoints[spawnIndex].transform.position.z);
        nView.RPC("MeshHide", RPCMode.All, true);
        health = 100;
        alive = true; 
        CheckStats(); 
    }

    #region Rpc Calls
    [RPC]
    void PlayerDied(string name)
    {
        List<string> dList = new List<string>
        {
            " was Decimated",
            " was Destroyed",
            " Had a Little Talking To...",
            " Met The Brutal End",
            " Got To The End Of The Road",
            " Was Pooffed",
            " Fell on Their Car Keys", 
            " Disapeared",
            " Got The Backhand",
            " Got Stabbed",
            " Ate S#!^, and Died",
            " Died",
            " Got the Bad End of the Gas Powered Stick", 
            " Never Stood a Chance",
            " Forgot To Tie Their Shoes",
            " Got Rejected", 
            " Went Bye-Bye",
            " Took a Potty Break",
            " Forgot How to Live", 
            " Died playing Derp-Bots",
            " Died because They Were Stupid, Only Stupid People Die",
            " Jumped Out of the 70th Story Window",
            " Tried To Be Happy", 
            " Thought They were Being Cleaver", 
            " Shat Themselves",
            " Ate Too Many Burritos",
            " Talked to Shanice", 
            " Couldn't be Bothered to Breath", 
            " Just had a Bad Day", 
            " Got an Headache",
            " Slipped in a Puddle",
            " Slipped on a Bannana Peel", 
            " Couldn't Handle Prison",
            " Droped The Soap", 
            " Got Hurt",
            " Got Dropped",
            " Gave Up on life",
            " Laid down",
            " Got Shot up",
            " Got messed up", 
            " Couldn't Dodge",
            " Fell ona Fork",
            " Got their Ankles Broke",
            " Got popped",
            " Sploded",
            " Got REKT",
            " Needs to Try Again",
            " Couldn't Find a Diplomatic Way",
            " Failed",
            " Fell off the Bike",
            " Sneezed Too Hard", 
            " Got a Shoe to The Head",
            " Pooted", 
            " Went Boom-Boom",
            " Got a really Bad Headache", 
            " Lost Their Glasses",
            " Got Samurai Sliced",
            " Got ###### In The ###",
            " Was Taken Advantaged Of",
            " Got up too fast",
            " Sat down too fast",
            " Found Love", 
            " Got Sat On", 
            " Realized That They Were Just a Pawn In Life's Unbeatable Game", 
            "'s Spegghetti Fell Out Of Their Pocket", 
            " Was Having Too Much Fun",
            " Was Found Guilty Of Trying", 
            " Met Santa",
            " Saw Slender",
            " Met The Easter Bunny",
            " Tried Too Hard", 
            " Was a Prick",
            " Was Found Guilty of Being a PissBoi", 
            " Yawned and Got Shot",
            " was Shot",
            " Couldn't Handle This Tail Whoopin'",
            " Broke Their Back", 
            " Failed the Test", 
            "'s Legs Fell Asleep", 
            " Never Found Their Way",
            " Faught the Persians",
            " Made the Wrong Life Choices", 
            " Met Justice", 
            " Saw Dat Booty", 
            " Is dead as Heck",
            " Got Jumped",
            " Got Punched in The Eye",
            " Wouldn't  Sing a Song",
            " Wouldn't Pay Up",
            " Had Until " + DateTime.Now.ToString(),
            " Had a good run",
            " Is no Longer With Us",
            " Got Wasted",
            " Is Dead... REJOICE!!!",
            " Lives on in Our Hearts and Minds",
            " Found Jesus",
            " Peed Too Hard",
            " Found Something Better To Do With Their Time",
            " Chose The Wrong Position",
            " Was too Frail To Walk",
            " Slammed Something Important In A Car Door",
            " Caught The Feels",
            " Got Their Feelings Hurt",
            " Was Offered Candy",
            " Took a Ting-Ting Into a Bing-Bing and Went Ding-Ding",
            " Faught JOOOHHHNNN CEENNNAAA!!! (and lost)",
            " Was Lasered", 
            " Got Death by One-Thousand Paper Cuts",
            " Didn't Check on Foxy", 
            " Forgot To Wind The Music Box", 
            " Ran out of Power", 
            " Faught CoryxKenshin(and Lost)", 
            " Asked Their Friends For Money",
            " Fell into a Deep Deprsion",
            " Is Hiding From The Mafia", 
            " Stepped on My Shoes",
            " Wouldn't Stop Talking", 
            " Couldn't Even",
            " Was Killed By Erectile Disfunction",
            " Dishonored Their Femuri",
            " Angered Half of Tumblr",
            " Questioned Religion",
            " Seen DA BOOTH At The Wrong Time"
        };



        nManager.PlayerDied(name + dList[UnityEngine.Random.Range(0, dList.Count)]); 
    }
    [RPC]
    void NetworkTakeDamageFromWeapon(int damage, NetworkMessageInfo info)
    {
        playerRecieved = info.networkView.GetComponent<NetworkView>(); 
        health = health - damage;
        CheckStats();
    }

    [RPC]
    void MeshHide(bool show, NetworkMessageInfo info)
    {
        info.networkView.GetComponent<MeshRenderer>().enabled = show;
    }
    [RPC]
    void Stasis()
    {
        StartCoroutine(TimeStasis());
    }
    IEnumerator TimeStasis()
    {
        stasis = true;
        yield return new WaitForSeconds(5);
        stasis = false;
    }

    [RPC]
    void Levitation(int duration)
    {
        StartCoroutine(LevitationTime(duration));
    }
    IEnumerator LevitationTime(int time)
    {

        
        rigidBody.useGravity = false;
        levitation = true; 
        yield return new WaitForSeconds(time);
        levitation = false; 
        rigidBody.useGravity = true;
        health = 0;
        CheckStats();



        Network.Instantiate(popExplosion, transform.position, Quaternion.identity, 0);
        Network.Instantiate(explosionPeices, transform.position, Quaternion.identity, 0);
    }

    [RPC]
    void SmokingEffect(bool shwEffect, NetworkMessageInfo info)
    {

        if (shwEffect)
        {
            info.networkView.GetComponent<ParticleSystem>().Play(); 
        }
        else
        {
            info.networkView.GetComponent<ParticleSystem>().Stop(); 
        }
    }
    #endregion

    #region NetworkSyncing
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        if (stream.isWriting)
        {

            syncPosition = rigidBody.position;
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

            syncStartPosition = rigidBody.position;
            syncEndPosition = syncPosition;

            qSrot = transform.rotation;
            qErot = rot;
        }


    }

    private void SyncedMovement()
    {
        syncTime += Time.deltaTime;
        rigidBody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);

        transform.rotation = Quaternion.Lerp(qSrot, qErot, syncTime / syncDelay);

    }
    #endregion
}
