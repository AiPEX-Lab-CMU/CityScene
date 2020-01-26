using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{

    public bool carsCanGo;
    public bool peopleCanWalk;
    public bool noTurnOnRed;

    TrafficSetController LightColor;


    // Start is called before the first frame update
    void Start()
    {
        LightColor = (TrafficSetController)this.GetComponent<TrafficSetController>();
    }

    //Truth table
    // cancarsgo = 1, canpeoplewalk = 0 -> cars running, people wait
    // cancarsgo = 0, canpeoplewalk = 1 -> cars waiting, people walking
    // For now, there are only states because cars are bidirectional. When cars start going in 4 directions, the number of states increase.


    // Update is called once per frame
    void Update()
    {
    }

    public void SetToGreen()
    {
        LightColor = (TrafficSetController)this.GetComponent<TrafficSetController>();
        if (LightColor.state != PhaseState.Go)
        {
            LightColor.state = PhaseState.Go;
            peopleCanWalk = false;
            carsCanGo = true;
        }
    }

    public void PedestriansCanWalk()
    {
        LightColor.state = PhaseState.Walk;
        peopleCanWalk = true;
    }

    public void PedestriansCannotWalk()
    {
        LightColor = (TrafficSetController)this.GetComponent<TrafficSetController>();
        LightColor.state = PhaseState.DontWalk;
        peopleCanWalk = false;
    }

    public void SetToYellow()
    {
        LightColor = (TrafficSetController)this.GetComponent<TrafficSetController>();
        if (LightColor.state != PhaseState.Warn)
        {
            LightColor.state = PhaseState.Warn;
            peopleCanWalk = false;
            carsCanGo = false;
        }
    }

    public void SetToRed()
    {
        LightColor = (TrafficSetController)this.GetComponent<TrafficSetController>();
        if (LightColor.state != PhaseState.Stop)
        {
            LightColor.state = PhaseState.Stop;
            peopleCanWalk = false;
            carsCanGo = false;
        }
    }

    public void StartLeftTurn()
    {
        /*
        LightColor = (TrafficSetController)this.GetComponent<TrafficSetController>();
        if (LightColor.state != PhaseState.Stop)
        {
            color.material.SetColor("_Color", Color.blue);
            peopleCanWalk = false;
            carsCanGo = false;
        }
        */
    }



}
