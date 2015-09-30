using UnityEngine;
using System.Collections;

public class SentryHealth : MonoBehaviour {
    [RequireComponent(typeof(LineRenderer))]
    NetworkView nView;
    float health;
    Transform bossPosition;
    Transform startPosition;
    public int startPos;

    RaycastHit hit;
    float range = 1000.0f;
    LineRenderer line;
    public Material lineMaterial;
    Vector3 direction; 
    bool alive; 
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
    // Use this for initialization
    void Start()
    {
        alive = true; 
        nView = GetComponent<NetworkView>();
        health = 500;
        bossPosition = GameObject.FindGameObjectWithTag("Boss").GetComponent<Transform>();

        line = GetComponent<LineRenderer>();
        line.SetVertexCount(2);
        line.material = lineMaterial;
        line.SetColors(Color.red, Color.red);
        line.SetWidth(0.1f, .25f);
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if (nView.isMine)
        {
            MainAi();
        }
        else
        {
            SyncedMovement(); 
        }
	}

    void MainAi()
    {
        if (startPos == 1)
        {
            startPosition = GameObject.FindGameObjectWithTag("Health1").GetComponent<Transform>();
            Vector3 startPosVector = new Vector3(startPosition.position.x, startPosition.position.y, startPosition.position.z);
            transform.position = Vector3.Lerp(transform.position, startPosVector, .5f * Time.deltaTime);
        }
        if (startPos == 2)
        {
            startPosition = GameObject.FindGameObjectWithTag("Health2").GetComponent<Transform>();
            Vector3 startPosVector = new Vector3(startPosition.position.x, startPosition.position.y, startPosition.position.z);
            transform.position = Vector3.Lerp(transform.position, startPosVector, .5f * Time.deltaTime);
        }


        
        //transform.Rotate(Vector3.one * 3 * Time.deltaTime);

        if (alive)
        {
            
            Ray ray = new Ray(transform.parent.GetComponent<Camera>().transform.position, transform.parent.GetComponent<Camera>().transform.forward);
            Vector3 pos = transform.position; 
            direction = (bossPosition.transform.position - transform.position).normalized;

            line.enabled = true;

            line.SetPosition(0, transform.position);
            line.SetPosition(1, bossPosition.position);
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
