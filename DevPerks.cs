using UnityEngine;
using System.Collections;

public class DevPerks : MonoBehaviour {
    
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
    private Quaternion lookRotation;
    private float rotationSpeed;
    Vector3 direction;
    NetworkManager nManager;
    NetworkView nView;
    Material mat; 
    // Use this for initialization
    void Start ()
    {
        mat = GetComponent<Material>(); 
        nView = GetComponent<NetworkView>(); 
        nManager = GameObject.FindGameObjectWithTag("GM/NM").GetComponent<NetworkManager>();
        if (nView.isMine)
        {
            if (nManager.playeName == "Danker")
            {
                nView.RPC("ShowDev", RPCMode.All, true);
            }
            else
            {
                nView.RPC("ShowDev", RPCMode.All, false);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (nView.isMine)
        {
            
        }
        else
        {
           // SyncedMovement();
        }

        Transform target = transform.parent;

        direction = (target.transform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);

        transform.Translate(Vector3.forward * 20 * Time.deltaTime);
        
        ColorChange(); 
        
    }
    void ColorChange()
    {
        Color color = new Color();
        color.r += (1 * Time.deltaTime);
        color.g += (1.5f * Time.deltaTime);
        color.r += (2 * Time.deltaTime);
        if (color.r >= 255)
        {
            color.r = 0;
        }
        if (color.g >= 255)
        {
            color.g = 255;
        }
        if (color.b >= 255)
        {
            color.b = 255;
        }
        mat.color = color; 
    }
    [RPC]
    void ShowDev(bool show, NetworkMessageInfo info)
    {
        MeshRenderer mRenderer = info.networkView.GetComponent<MeshRenderer>();
        mRenderer.enabled = show; 
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
