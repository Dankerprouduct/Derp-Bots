using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _GM : MonoBehaviour {

	

    List<PointSystem> players = new List<PointSystem>();


   // PointSystem pSystem; 
    // Use this for initialization
    NetworkView nView; 

    LineRenderer line;
    public Material lineMaterial;
    RaycastHit hit;
    float range = 1000.0f;

    public GameObject explosionPrefab;

    List<string> deaths = new List<string>(); 

    void Start()
    {
        nView = GetComponent<NetworkView>(); 
       // pSystem = new PointSystem();

        line = GetComponent<LineRenderer>();
        line.SetVertexCount(2);
        line.material = lineMaterial;
        line.SetColors(Color.red, Color.red);
        line.SetWidth(0.1f, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    void OnGUI()
    {
        Notifications(); 
    }
    void Notifications()
    {
        

        for (int i = 0; i < deaths.Count; i++)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + i * 10, 100, 50), deaths[i].ToString());
        }
        if (deaths.Count > 3)
        {
            deaths.RemoveAt(0);
        }

    }
    [RPC]
    void Died(string sentUsername)
    {
        // send rpc all username + isdead
     //   Debug.Log("Yes. im beinh called");
    //    Debug.Log(sentUsername + " died");
        deaths.Add(sentUsername + " Died!");
    }

    [RPC]
    void Shot(Vector3 position, Vector3 direction, bool show)
    {
        print("lets see if this is needed");
       Ray ray = new Ray(position, direction);

        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hit.point + hit.normal);

        if (Physics.Raycast(ray, out hit, range))
        {
            if (show)
            {
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
    void Explosion(Vector3 pos)
    {
        Instantiate(explosionPrefab, pos, Quaternion.identity); 
    }


}
