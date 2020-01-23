using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTry : MonoBehaviour
{
    // Start is called before the first frame update

    public bool randomWalking;

    public enum TypeOfObject { Car, Person };

    public TypeOfObject thisType;

    public List<Transform> wayPoints;

    NavMeshAgent agent;

    Random waypointChooser;

    public bool waitingForTrafficLight;

    TrafficIntersection encounteredIntersection;

    NavMeshTry carInFront;

    int index;

    //bool hitsCenterLane;

    void Start()
    {
        encounteredIntersection = null;
        waitingForTrafficLight = false;
        Random.seed = System.DateTime.Now.Millisecond;
        waypointChooser = new Random();
        thisType = TypeOfObject.Car;
        agent = GetComponent<NavMeshAgent>();
        if (randomWalking)
            index = Random.Range(0, 100) % (wayPoints.Count - 1);
        else
            index = 0;
        agent.destination = wayPoints[index].position;
        //hitsCenterLane = false;
    }

    private IEnumerator WaitForStopSign()
    {
        UnityEngine.Debug.Log("Encountered a stop sign");
        bool status = agent.isStopped;
        agent.isStopped = true;
        yield return new WaitForSeconds(3.0f);
        UnityEngine.Debug.Log("Waited for 3 seconds");
        agent.isStopped = status;
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.CompareTag("StopSign"))
        {
            StartCoroutine(WaitForStopSign());
        }

        TrafficLightManager t = other.gameObject.GetComponent<TrafficLightManager>();

        if (t)
        {
            if(!agent.isStopped && !t.carsCanGo && encounteredIntersection != other.gameObject.transform.parent.gameObject.GetComponent<TrafficIntersection>())
            {
                UnityEngine.Debug.Log("Intersection is different, should listen now");
                encounteredIntersection = other.gameObject.transform.parent.gameObject.GetComponent<TrafficIntersection>();
                if(!t.noTurnOnRed && Vector3.Distance(agent.steeringTarget, agent.transform.position) < 0.5f)
                {
                    /* Determine if it is turning */
                    UnityEngine.Debug.Log("Should be turning now");
                    Vector3 turningVector = agent.steeringTarget - agent.transform.position;
                    /* If turning left then the car still has to wait for green light */
                    if (LeftTurn())
                    {
                        UnityEngine.Debug.Log("Turning left");
                        waitingForTrafficLight = true;
                        agent.isStopped = !t.carsCanGo;
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Turning right");
                    }
                }
                else
                {
                    waitingForTrafficLight = true;
                    agent.isStopped = !t.carsCanGo;
                }
            }
            return;
        }

        NavMeshTry nmt = other.gameObject.GetComponent<NavMeshTry>();
        if(nmt)
        {
            if (nmt.agent.isStopped  && !agent.isStopped)
            {
                UnityEngine.Debug.Log("The car at front has stopped");
                if(nmt.waitingForTrafficLight && !waitingForTrafficLight)
                    waitingForTrafficLight = true;
                agent.isStopped = true;
                carInFront = nmt;
            }
            return;
        }
    }

    /* Wait for Traffic Light */
    private void OnTriggerStay(Collider other)
    {
        TrafficLightManager t = other.gameObject.GetComponent<TrafficLightManager>();

        if (t && agent.isStopped)
        {
            agent.isStopped = !t.carsCanGo;
            if (!agent.isStopped)
                waitingForTrafficLight = false;
        }

        /*if(other.gameObject.CompareTag("CenterLane") && Vector3.Distance(agent.transform.position, other.gameObject.transform.position) < 0.1f)
        {
            hitsCenterLane = true;
        }*/
    }


    /* When the car is in a queue waiting for a traffic light and the car in front of it starts going, it should also start going */
    private void OnTriggerExit(Collider other)
    {
        NavMeshTry nmt = other.gameObject.GetComponent<NavMeshTry>();
        if(nmt)
        {
            if(agent.isStopped && nmt == carInFront)
            {
                UnityEngine.Debug.Log("The car at front has moved");
                if(waitingForTrafficLight)
                    waitingForTrafficLight = false;
                carInFront = null;
                agent.isStopped = false;
            }
        }
        /*
        if (other.gameObject.CompareTag("CenterLane"))
        {
            hitsCenterLane = false;
        }*/
    }

    void Update()
    {
        if (Vector3.Distance(agent.transform.position, agent.destination) < 0.1f)
        {
            UnityEngine.Debug.Log("Destination is reached");
            Random.seed = System.DateTime.Now.Millisecond;
            if (randomWalking)
                index = Random.Range(0, 100) % (wayPoints.Count - 1);
            else
                index++;
            UnityEngine.Debug.Log("Waypoint " + index.ToString() + " is chosen");
            agent.destination = wayPoints[index].position;
        }

    }

    public void CarStop()
    {
        agent.isStopped = true;
    }

    public void CarGo()
    {
        agent.isStopped = false;
    }

    bool LeftTurn()
    {
        Vector3 turningVector = agent.steeringTarget - agent.transform.position;
        if (Vector3.SignedAngle(agent.transform.position, turningVector, Vector3.up) < 0)
            return true;
        return false;
    }

    bool RightTurn()
    {
        Vector3 turningVector = agent.steeringTarget - agent.transform.position;
        if (Vector3.SignedAngle(agent.transform.position, turningVector, Vector3.up) < 0)
            return true;
        return false;
    }
}
