﻿using System.Collections;
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



    public bool shouldStartMove = false;

    [SerializeField] float beginDelay;

    [SerializeField] Animator animator;
    [SerializeField] bool debugPathDraw;

    LineRenderer lineDraw;

    /* Boolean showing if the pedestrian is waiting for traffic light */
    public bool waitingForTrafficLight;

    AnimationStateController animState;


    //Bool used for people who are standing in a group at the start
    public bool isStanding = false;
    [SerializeField] Transform standPoint;
    [SerializeField] Transform groupCentre;

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

        if (thisType == TypeOfObject.Person)
            animState = this.gameObject.GetComponent<AnimationStateController>();


        //Setting shouldStartMove = false for those people who need to be standing at the start
        if (thisType == TypeOfObject.Person && isStanding)
        {
            shouldStartMove = false;

            wayPoints.Add(standPoint);

        }

        //Using invoke to implement the start move delay
        Invoke("StartMove", beginDelay);

    }

    // Update is called once per frame
    void Update()
    {
        
        if(thisType == TypeOfObject.Person)
        {

            if (shouldMove && !isStanding)
            {

                animState.animationState = AnimationStateController.HumanoidStates.Walk;

            }

            if(!shouldMove || isStanding)
            {

                animState.animationState = AnimationStateController.HumanoidStates.Idle;

            }

        }

        if (Vector3.Distance(this.transform.position, currentTarget.position) < 0.1f)
        {

            if(currentTarget == standPoint)
            { //this will be true when the person finishes a cycle of the waypoints and comes back
                //At this point the person should 'rejoin the group' i.e (stop moving and face the direction of group centre

                shouldStartMove = false;

                this.transform.LookAt(groupCentre);

                //restart the entire cycle
                Invoke("StartMove", beginDelay);
            }

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

        if (!shouldMove || !shouldStartMove)
            return;

        this.transform.LookAt(currentTarget);

        this.transform.position += this.transform.forward * Time.deltaTime * movespeed;

    }

    private void OnTriggerEnter(Collider other)
    {
        TrafficLightManager t = other.gameObject.GetComponent<TrafficLightManager>();

        if (t)
        {
            //if(thisType == TypeOfObject.Person)
            //    shouldMove = t.peopleCanWalk;
            //    waitingForTrafficLight = !t.peopleCanWalk;

            if (thisType == TypeOfObject.Car)
                shouldMove = t.carsCanGo;

        }

    }

    void StartMove()
    {

        shouldStartMove = true;

    }

}
