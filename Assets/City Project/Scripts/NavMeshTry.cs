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

    Stopwatch stopWatch;

    string carID;

    SendMessage messageSender;

    HashSet<GameObject> pedestrians;

    void Start()
    {
        pedestrians = new HashSet<GameObject>();
        carID = gameObject.name.Substring(3);
        stopWatch = new Stopwatch();
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
        messageSender = (SendMessage)GameObject.Find("messageSender").GetComponent<SendMessage>();
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
            if(encounteredIntersection != other.gameObject.transform.parent.gameObject.GetComponent<TrafficIntersection>())
            {
                encounteredIntersection = other.gameObject.transform.parent.gameObject.GetComponent<TrafficIntersection>();
                if (!agent.isStopped && !t.carsCanGo)
                {
                    UnityEngine.Debug.Log("Intersection is different, should listen now");
                    if (!t.noTurnOnRed && Vector3.Distance(agent.steeringTarget, agent.transform.position) < 0.5f)
                    {
                        /* Determine if it is turning */
                        UnityEngine.Debug.Log("Should be turning now");
                        Vector3 turningVector = agent.steeringTarget - agent.transform.position;
                        /* If turning left then the car still has to wait for green light */
                        if (LeftTurn())
                        {
                            UnityEngine.Debug.Log("Turning left");
                            stopWatch.Reset();
                            waitingForTrafficLight = true;
                            agent.isStopped = !t.carsCanGo;
                            stopWatch.Start();
                        }
                        else
                        {
                            UnityEngine.Debug.Log("Turning right");
                        }
                    }
                    else
                    {
                        stopWatch.Reset();
                        waitingForTrafficLight = true;
                        agent.isStopped = !t.carsCanGo;
                        stopWatch.Start();
                    }
                }
            }
            return;
        }

        NavMeshTry nmt = other.gameObject.GetComponent<NavMeshTry>();
        if(nmt)
        {
            if (nmt.agent.isStopped  && !agent.isStopped)
            {
                if (Vector3.Angle(this.transform.forward, nmt.gameObject.transform.forward) < 90.0f)
                {
                    UnityEngine.Debug.Log("The car at front has stopped");
                    if (nmt.waitingForTrafficLight && !waitingForTrafficLight)
                        waitingForTrafficLight = true;
                    agent.isStopped = true;
                    carInFront = nmt;
                }
                return;
            }

            return;
        }
    }

    /* Wait for Traffic Light */
    private void OnTriggerStay(Collider other)
    {
        TrafficLightManager t = other.gameObject.GetComponent<TrafficLightManager>();

        if (t && waitingForTrafficLight && agent.isStopped == true)
        {
            agent.isStopped = !t.carsCanGo;
            if (!agent.isStopped)
            {
                waitingForTrafficLight = false;
                stopWatch.Stop();
                string intersectionID = t.transform.parent.name.Substring(13);
                string lightID = other.gameObject.name.Substring(12);
                string result = carID + "#" + intersectionID + "#" + lightID + "#" + stopWatch.Elapsed.TotalMilliseconds.ToString();
                messageSender.sendBytes("003", result);
            }
        }
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
                index = (index + 1) % (wayPoints.Count - 1);
            UnityEngine.Debug.Log("Waypoint " + index.ToString() + " is chosen");
            agent.destination = wayPoints[index].position;
        }

    }

    public bool IsStopped()
    {
        return agent.isStopped;
    }

    public void AddPedestrian(GameObject gameobj)
    {
        pedestrians.Add(gameobj);
        agent.isStopped = true;
    }

    public void RemovePedestrian(GameObject gameobj)
    {
        if(pedestrians.Contains(gameobj))
            pedestrians.Remove(gameobj);
    }

    public bool InCarRange(GameObject gameobj)
    {
        return pedestrians.Contains(gameobj);
    }

    public void CarGo()
    {
        /* Only go if no people in front and not waiting for traffic light */
        if(!waitingForTrafficLight)
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
