using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficIntersection : MonoBehaviour
{
    // Start is called before the first frame update
    public bool trafficLight1;
    public bool trafficLight2;
    public bool trafficLight3;
    public bool trafficLight4;

    public float carPassDuration1;
    public float carPassDuration2;

    public float peopleWalkDuration;

    public bool usePresetValue1;
    public bool usePresetValue2;

    public bool generateRandomValue1;
    public bool generateRandomValue2;

    public bool usePresetValuePeople;
    public bool generateRandomValuePeople;

    public bool direction1LeftTurn;
    public bool direction2LeftTurn;

    public float direction1LeftTurnDuration;
    public float direction2LeftTurnDirection;

    private enum TrafficStatus { Direction1, Direction1LeftTurn, Direction2, Direction2LeftTurn, Pedestrians };

    private TrafficStatus state;

    float timer;

    float colorChangeTimer;


    List<TrafficLightManager> direction1;
    List<TrafficLightManager> direction2;
    void Start()
    {
        direction1 = new List<TrafficLightManager>();
        direction2 = new List<TrafficLightManager>();
        if(this.gameObject.transform.Find("TrafficLight1").gameObject.GetComponent<TrafficLightManager>())
        {
            Debug.Log("Exists");
        }
        if (trafficLight1)
            direction1.Add(this.gameObject.transform.Find("TrafficLight1").gameObject.GetComponent<TrafficLightManager>());
        if (trafficLight2)
            direction1.Add(this.gameObject.transform.Find("TrafficLight2").gameObject.GetComponent<TrafficLightManager>());
        if (trafficLight3)
            direction2.Add(this.gameObject.transform.Find("TrafficLight3").gameObject.GetComponent<TrafficLightManager>());
        if (trafficLight4)
            direction2.Add(this.gameObject.transform.Find("TrafficLight4").gameObject.GetComponent<TrafficLightManager>());

        TrafficLightInfo tInfo = GameObject.FindObjectOfType<TrafficLightInfo>();
        if (usePresetValue1)
            carPassDuration1 = tInfo.carPassDuration;
        else if (generateRandomValue1)
            carPassDuration1 = Random.Range(5.0f, 15.0f);
        if (usePresetValue2)
            carPassDuration2 = tInfo.carPassDuration;
        else if (generateRandomValue2)
            carPassDuration2 = Random.Range(5.0f, 15.0f);
        if (usePresetValuePeople)
            peopleWalkDuration = tInfo.peopleWalkDuration;
        else if (generateRandomValuePeople)
            peopleWalkDuration = Random.Range(5.0f, 15.0f);
        state = TrafficStatus.Direction1;
        foreach(TrafficLightManager manager in direction1)
        {
            manager.PedestriansCannotWalk();
            manager.SetToGreen();
        }
        foreach (TrafficLightManager manager in direction2)
        {
            manager.PedestriansCannotWalk();
            manager.SetToRed();
        }

        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        switch (state)
        {
            case TrafficStatus.Direction1:
                if (timer > carPassDuration1)
                {
                    foreach (TrafficLightManager manager in direction1)
                    {
                        manager.SetToYellow();
                    }
                    colorChangeTimer += Time.deltaTime;
                    if (colorChangeTimer > 3.0f)
                    {
                        colorChangeTimer = 0.0f;
                        foreach (TrafficLightManager manager in direction1)
                        {
                            manager.SetToRed();
                        }
                        if (!direction1LeftTurn)
                        {
                            foreach (TrafficLightManager manager in direction2)
                            {
                                manager.SetToGreen();
                            }
                            timer = 0.0f;
                            state = TrafficStatus.Direction2;
                        }
                        else
                        {
                            foreach (TrafficLightManager manager in direction1)
                            {
                                manager.StartLeftTurn();
                            }
                            timer = 0.0f;
                            state = TrafficStatus.Direction1LeftTurn;
                        }
                    }
                }
                break;
            case TrafficStatus.Direction1LeftTurn:
                if(timer > direction1LeftTurnDuration)
                {
                    foreach (TrafficLightManager manager in direction1)
                    {
                        manager.SetToRed();
                    }
                    foreach (TrafficLightManager manager in direction2)
                    {
                        manager.SetToGreen();
                    }
                    timer = 0.0f;
                    state = TrafficStatus.Direction2;
                }
                break;
            case TrafficStatus.Direction2:
                if (timer > carPassDuration2)
                {
                    foreach (TrafficLightManager manager in direction2)
                    {
                        manager.SetToYellow();
                    }
                    colorChangeTimer += Time.deltaTime;
                    if (colorChangeTimer > 3.0f)
                    {
                        colorChangeTimer = 0.0f;
                        foreach (TrafficLightManager manager in direction2)
                        {
                            manager.SetToRed();
                        }
                        timer = 0.0f;
                        if (!direction2LeftTurn)
                        {
                            foreach (TrafficLightManager manager in direction1)
                            {
                                manager.PedestriansCanWalk();
                            }
                            foreach (TrafficLightManager manager in direction2)
                            {
                                manager.PedestriansCanWalk();
                            }
                            state = TrafficStatus.Pedestrians;
                        }
                        else
                        {
                            foreach (TrafficLightManager manager in direction2)
                            {
                                manager.StartLeftTurn();
                            }
                            state = TrafficStatus.Direction2LeftTurn;
                        }
                    }

                }
                break;
            case TrafficStatus.Direction2LeftTurn:
                foreach (TrafficLightManager manager in direction2)
                {
                    manager.SetToRed();
                }
                timer = 0.0f;
                foreach (TrafficLightManager manager in direction1)
                {
                    manager.PedestriansCanWalk();
                }
                foreach (TrafficLightManager manager in direction2)
                {
                    manager.PedestriansCanWalk();
                }
                state = TrafficStatus.Pedestrians;
                break;
            case TrafficStatus.Pedestrians:
                if (timer > peopleWalkDuration)
                {
                    timer = 0.0f;
                    foreach (TrafficLightManager manager in direction1)
                    {
                        manager.PedestriansCannotWalk();
                        manager.SetToGreen();
                    }
                    foreach (TrafficLightManager manager in direction2)
                    {
                        manager.PedestriansCannotWalk();
                    }
                    state = TrafficStatus.Direction1;
                }
                break;
            default:
                break;
        }
    }
}
