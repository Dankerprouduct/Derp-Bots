using UnityEngine;
using System.Collections;

public class DeadEffect : MonoBehaviour {

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    private Quaternion roatation = Quaternion.identity;
    private Quaternion qSrot = Quaternion.identity;
    private Quaternion qErot = Quaternion.identity;

    Rigidbody rigidBody;

	// Use this for initialization
    NetworkView nview; 
    Vector3 randomDirection;
    NetworkManager nManager;
    Color pColor;
    public Material mat; 
	void Start ()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            nview = GetComponent<NetworkView>();
            nManager = GameObject.FindGameObjectWithTag("GM/NM").GetComponent<NetworkManager>();
            pColor = new Color(nManager.vColor.x, nManager.vColor.y, nManager.vColor.z);

            GetComponent<MeshRenderer>().material.color = pColor;

            nview.RPC("DeathColor", RPCMode.All, nManager.vColor);
        }
        rigidBody = GetComponent<Rigidbody>();
        randomDirection = new Vector3(Random.value, Random.value, Random.value);
        GetComponent<Rigidbody>().AddForce(randomDirection * 250);

        StartCoroutine(WaitToDespawn()); 
        
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
    IEnumerator WaitToDespawn()
    {
        yield return new WaitForSeconds(15);
        Network.Destroy(this.gameObject); 
    }
    [RPC]
    void DeathColor(Vector3 color, NetworkMessageInfo info)
    {

        MeshRenderer otherPlayerRenderer = info.networkView.GetComponent<MeshRenderer>();

        Material otherPlayerMaterial = info.networkView.GetComponent<Material>();
        Color nColor = new Color(color.x, color.y, color.z);

        otherPlayerRenderer.material.color = nColor;



    }

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
}
