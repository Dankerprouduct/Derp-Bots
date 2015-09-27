using UnityEngine;
using System.Collections;

public class Hit : MonoBehaviour
{

    public GUISkin skin;

    NetworkManager nManager;
    int health = 100;
    // Use this for initialization
    void Start()
    {
        nManager = GameObject.FindGameObjectWithTag("Network").GetComponent<NetworkManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
