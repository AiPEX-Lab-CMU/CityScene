using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonPrimitiveAI : MonoBehaviour
{
    WaypointMovement wm;


    // Start is called before the first frame update
    void Start()
    {
        wm = this.gameObject.GetComponent<WaypointMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        //raycast to see if there is a car in front of it
        // if there is, then check the distance. If the distance is close enough, stop moving.

        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 100.0f))
        {

            if (hit.collider.gameObject.CompareTag("Car"))
            {
                //there is a car in front of this person, now need to check distance
                /* If the car is already waiting for the person then the person don't need to stop, just pass through quickly */
                if (hit.distance < 1.0f && !hit.collider.gameObject.GetComponent<NavMeshTry>().InCarRange(this.gameObject))
                {
                    wm.shouldMove = false;

                    goto PointA;

                }



            }

        }
        /* If not waiting for traffic light, then the person should keep walking */
        if (!wm.waitingForTrafficLight)
            wm.shouldMove = true;

    PointA:

        return;
    }
}
