using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    public enum TypeOfObject { Car, Person};

    public TypeOfObject thisType;

    public List<Transform> wayPoints;

    [SerializeField]
    Transform currentTarget;

    public float movespeed;

    //Keeping a track of the current waypoint the target needs to go towards.
    int waypointCounter = 0;

    public bool shouldMove = true;

    [SerializeField] Animator animator;
    [SerializeField] bool debugPathDraw;

    LineRenderer lineDraw;

    /* Boolean showing if the pedestrian is waiting for traffic light */
    public bool waitingForTrafficLight;

    // Start is called before the first frame update
    void Start()
    {
        waitingForTrafficLight = false;

        currentTarget = wayPoints[waypointCounter];

        lineDraw = this.gameObject.GetComponent<LineRenderer>();

        if (debugPathDraw)
        {
            lineDraw.positionCount = wayPoints.Count;

            for(int i =0; i< wayPoints.Count; i++)
            {

                lineDraw.SetPosition(i, wayPoints[i].position);

            }


        } else
        {
            if(lineDraw)
                lineDraw.enabled = false;

        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if(thisType == TypeOfObject.Person)
        {

            if (shouldMove)
            {

                //animator.SetBool("shouldMove", true);

            } else
            {

               // animator.SetBool("shouldMove", false);

            }

        }

        if (Vector3.Distance(this.transform.position, currentTarget.position) < 0.1f)
        {

                //The waypoint needs to change once the target reaches the current waypoint it was going towards
                waypointCounter++;

                if (waypointCounter > wayPoints.Count - 1)
                    waypointCounter = 0;

                currentTarget = wayPoints[waypointCounter];
            
        } else
        {

            MoveMe();

        }


    }

    void MoveMe()
    {

        if (!shouldMove)
            return;

        this.transform.LookAt(currentTarget);

        this.transform.position += this.transform.forward * Time.deltaTime * movespeed;

    }

    private void OnTriggerStay(Collider other)
    {
        TrafficLightManager t = other.gameObject.GetComponent<TrafficLightManager>();

        if (t)
        {
            if(thisType == TypeOfObject.Person)
                shouldMove = t.peopleCanWalk;
                waitingForTrafficLight = t.peopleCanWalk;

            if (thisType == TypeOfObject.Car)
                shouldMove = t.carsCanGo;

        }

    }


}
