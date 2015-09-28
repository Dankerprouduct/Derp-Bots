using UnityEngine;
using System.Collections;

public class Companion : MonoBehaviour {

    // Use this for initialization
    public GameObject explosion; 
    GameObject[] players;
    int playerIndex;  
    private Quaternion lookRotation;
    private float rotationSpeed;
    Vector3 direction;
    NetworkView nView;
    NetworkView pView; 
    bool show;

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
    void Start ()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        playerIndex = Mathf.RoundToInt(Random.Range(0, players.Length));
        nView = GetComponent<NetworkView>();

        GameObject.Destroy(gameObject, 15); 
        //GetComponent<MeshRenderer>().enabled = false; 
    }
	
	// Update is called once per frame
	void Update ()
    {
        Transform target = players[playerIndex].transform;

        direction = (target.transform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);

        transform.Translate(Vector3.forward * 20 * Time.deltaTime);

    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            pView = other.GetComponent<NetworkView>();

           // pView.RPC("NetworkTakeDamageFromWeapon", pView.owner, 5);
            if (pView.isMine)
            {
                PlayerMovement pMovement;
                pMovement = other.GetComponent<PlayerMovement>();
                pMovement.ClientTakeDamageFromWeapon(5);
            }
            else
            {
                pView.RPC("NetworkTakeDamageFromWeapon", pView.owner, 5);
            }
            Network.Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, 0);
            Destroy(this.gameObject); 
        }
    }
    [RPC]
    void NetworkTakeDamageFromWeapon(int damage, NetworkMessageInfo info)
    {
        Network.Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, 0);
        Destroy(this.gameObject);
    }
    public void ClientTakeDamageFromWeapon()
    {
        Network.Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, 0);
        Destroy(this.gameObject);
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
