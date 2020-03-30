using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapshot : MonoBehaviour
{
    // Start is called before the first frame update
    public float distance;
    public GameObject center;
    public Snapshot(float distance, GameObject center)
    {
        this.distance = distance;
        this.center = center;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
