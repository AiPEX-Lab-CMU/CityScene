using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{

    public bool canPeopleWalk;
    public bool canCarsGo;

    [SerializeField] float peopleWalkTimer;
    [SerializeField] float carsRunningTimer;

    TrafficLightInfo trafficLightInfo;

    // Start is called before the first frame update
    void Start()
    {

        trafficLightInfo = GameObject.FindObjectOfType<TrafficLightInfo>();

        canCarsGo = true;
        canPeopleWalk = false;

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

        if(carsRunningTimer > trafficLightInfo.carsPassDuration)
        {
            canCarsGo = false;
            canPeopleWalk = true;
            carsRunningTimer = 0.0f;
        }

        if(peopleWalkTimer > trafficLightInfo.peopleWalkDuration)
        {
            canPeopleWalk = false;
            canCarsGo = true;
            peopleWalkTimer = 0.0f;
        }

    }



}
