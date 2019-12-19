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

        if(Physics.Raycast(this.transform.position, this.transform.forward, out hit, 100.0f))
        {

            if (hit.collider.gameObject.CompareTag("Car"))
            {
                //there is a car in front of this person, now need to check distance
                if(hit.distance < 1.0f)
                {

                    wm.shouldMove = false;

                    goto PointA;

                }

                

            }

        }
        wm.shouldMove = true;

        PointA:

        return;
    }
}
