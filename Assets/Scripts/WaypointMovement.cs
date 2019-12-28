using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    public enum TypeOfObject { Car, Person };

    public TypeOfObject thisType;

    public List<Transform> wayPoints;

    [SerializeField]
    Transform currentTarget;

    [SerializeField] float beginDelay = 0.0f;

    public float movespeed;

    //Keeping a track of the current waypoint the target needs to go towards.
    int waypointCounter = 0;

    //controls movement after movement has been activated
    public bool shouldMove;
    //controls activation of movement
    bool shouldStartMove = false;

    [SerializeField] Animator animator;
    [SerializeField] bool debugPathDraw;

    LineRenderer lineDraw;

    Stopwatch stopwatch;

    bool timerStopped;

    // Start is called before the first frame update
    void Start()
    {
        timerStopped = false;

        stopwatch = new Stopwatch();

        currentTarget = wayPoints[waypointCounter];

        //Movement only starts after the begin delay

        shouldStartMove = false;
        Invoke("StartMoving", beginDelay);

        lineDraw = this.gameObject.GetComponent<LineRenderer>();

        if (lineDraw)
        {
            lineDraw.positionCount = wayPoints.Count;

            for (int i = 0; i < wayPoints.Count; i++)
            {

                lineDraw.SetPosition(i, wayPoints[i].position);

            }

            lineDraw.enabled = debugPathDraw;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (thisType == TypeOfObject.Person)
        {
            lineDraw.enabled = debugPathDraw;

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

        if (!shouldMove || !shouldStartMove)
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
                shouldMove = t.canPeopleWalk;

            if (thisType == TypeOfObject.Car)
                shouldMove = t.canCarsGo;

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collided = other.gameObject;
        if(collided.name == "Traffic_Light_5")
        {
            UnityEngine.Debug.Log("Start timing");
            stopwatch.Start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject collided = other.gameObject;
        if(collided.name == "Traffic_Light_6" && !timerStopped)
        {
            UnityEngine.Debug.Log("Stop timing");
            stopwatch.Stop();
            SendMessage messageSender = (SendMessage)GameObject.Find("messageSender").GetComponent(typeof(SendMessage));
            messageSender.sendBytes("003", stopwatch.Elapsed.TotalMilliseconds.ToString());
            timerStopped = !timerStopped;
        }
    }

    void StartMoving()
    {

        shouldStartMove = true;

    }

}
