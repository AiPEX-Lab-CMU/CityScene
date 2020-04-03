using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapshot : MonoBehaviour
{
    // Start is called before the first frame update
    public float distance;
    public Vector3 center;
    public bool visited = false;
    public Snapshot(float distance, Vector3 center)
    {
        this.distance = distance;
        this.center = center;
        this.visited = false;
    }

}
