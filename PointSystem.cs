using UnityEngine;
using System.Collections;


public class PointSystem : MonoBehaviour {


    public NetworkView nView;
    public int point;

    public PointSystem(NetworkView player, int pnt)
    {
        nView = player;
        point = pnt; 

    }
    public PointSystem()
    {

    }
    public void AddPoint()
    {
        point++; 
    }

}
