using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugLineDraw : MonoBehaviour
{
    [SerializeField] GameObject waypointParent;

    List<Transform> wayPoints;

    [SerializeField] bool drawPathLine = false;

    [SerializeField] float totalPathDistance;

    LineRenderer lineRend;

    // Start is called before the first frame update
    void Start()
    {

        lineRend = this.gameObject.GetComponent<LineRenderer>();

        wayPoints = new List<Transform>();

        //making this zero at the start to avoid carry overs
        totalPathDistance = 0;

        //the waypoint list needs to be populated using the child transforms of the waypointParent GO
        for(int i = 0; i < waypointParent.transform.childCount; i++)
        {
            wayPoints.Add(waypointParent.transform.GetChild(i));

            //calculate the total distance
            if (i > 0)
            {

                totalPathDistance += Vector3.Distance(wayPoints[i].position, wayPoints[i - 1].position);

            }
        }

        //Set positions in the line renderer
        lineRend.positionCount = wayPoints.Count;
        for (int i = 0; i < wayPoints.Count; i++)
        {

            lineRend.SetPosition(i, wayPoints[i].position);

        }


    }

    // Update is called once per frame
    void Update()
    {

        lineRend.enabled = drawPathLine;

    }
}
