using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{

    public bool canPeopleWalk;
    public bool canCarsGo;
    public bool usePresetValue;
    public bool generateRandomValue;
    public float carsPassDuration;
    public float peopleWalkDuration;

    Renderer color;

    [SerializeField] float peopleWalkTimer;
    [SerializeField] float carsRunningTimer;

    TrafficLightInfo trafficLightInfo;

    // Start is called before the first frame update
    void Start()
    {
        color = this.GetComponent<Renderer>();
        color.material.SetColor("_Color", Color.green);
        peopleWalkTimer = 0.0f;
        carsRunningTimer = 0.0f;
        Debug.Log("Start");
        trafficLightInfo = GameObject.FindObjectOfType<TrafficLightInfo>();
        canCarsGo = true;
        canPeopleWalk = false;
        if(usePresetValue)
        {
            carsPassDuration = trafficLightInfo.carsPassDuration;
            peopleWalkDuration = trafficLightInfo.peopleWalkDuration;
        }
        else
        {
            System.Random rnd = new System.Random();
            if(generateRandomValue)
            {
                carsPassDuration = rnd.Next(5, 15);
                peopleWalkDuration = rnd.Next(5, 15);
            }
        }
    }

    //Truth table
    // cancarsgo = 1, canpeoplewalk = 0 -> cars running, people wait
    // cancarsgo = 0, canpeoplewalk = 1 -> cars waiting, people walking
    // For now, there are only states because cars are bidirectional. When cars start going in 4 directions, the number of states increase.


    // Update is called once per frame
    void Update()
    {
        if (canCarsGo)
            carsRunningTimer += Time.deltaTime;

        if (canPeopleWalk)
            peopleWalkTimer += Time.deltaTime;

        if(carsRunningTimer > carsPassDuration)
        {
            canCarsGo = false;
            canPeopleWalk = true;
            peopleWalkTimer = 0.0f;
            carsRunningTimer = 0.0f;
            color.material.SetColor("_Color", Color.red);
        }

        if(peopleWalkTimer > peopleWalkDuration)
        {
            canPeopleWalk = false;
            canCarsGo = true;
            carsRunningTimer = 0.0f;
            peopleWalkTimer = 0.0f;
            color.material.SetColor("_Color", Color.green);
        }

    }



}
