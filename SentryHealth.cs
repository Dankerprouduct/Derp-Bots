using UnityEngine;
using System.Collections;

public class SentryHealth : MonoBehaviour {

    NetworkView nView;
    float health;
    Transform bossPosition;
    Transform startPosition;
    public int startPos;
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
        nView.GetComponent<NetworkView>();
        health = 500;
        bossPosition = GameObject.FindGameObjectWithTag("Boss").GetComponent<Transform>();

        
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
            startPosition = GameObject.Find("SentryHealthPosition1").GetComponent<Transform>();
        }
        if (startPos == 2)
        {
            startPosition = GameObject.Find("SentryHealthPosition2").GetComponent<Transform>();
        }
        Vector3 startPosVector = new Vector3(startPosition.position.x, startPosition.position.y, startPosition.position.z);
        transform.position = Vector3.Lerp(transform.position, startPosVector, 2 * Time.deltaTime);
        //transform.Rotate(Vector3.one * 3 * Time.deltaTime);


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
