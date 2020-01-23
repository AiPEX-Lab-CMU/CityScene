using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{

    public bool carsCanGo;
    public bool peopleCanWalk;
    public bool noTurnOnRed;

    Renderer color;


    // Start is called before the first frame update
    void Start()
    {
        color = this.GetComponent<Renderer>();
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
        color = this.GetComponent<Renderer>();
        if (color.material.color != Color.green)
        {
            color.material.SetColor("_Color", Color.green);
            peopleCanWalk = false;
            carsCanGo = true;
        }
    }

    public void PedestriansCanWalk()
    {
        peopleCanWalk = true;
    }

    public void PedestriansCannotWalk()
    {
        peopleCanWalk = false;
    }

    public void SetToYellow()
    {
        color = this.GetComponent<Renderer>();
        if (color.material.color != Color.yellow)
        {
            color.material.SetColor("_Color", Color.yellow);
            peopleCanWalk = false;
            carsCanGo = false;
        }
    }

    public void SetToRed()
    {
        color = this.GetComponent<Renderer>();
        if (color.material.color != Color.red)
        {
            color.material.SetColor("_Color", Color.red);
            peopleCanWalk = false;
            carsCanGo = false;
        }
    }

    public void StartLeftTurn()
    {
        color = this.GetComponent<Renderer>();
        if(color.material.color != Color.blue)
        {
            color.material.SetColor("_Color", Color.blue);
            peopleCanWalk = false;
            carsCanGo = false;
        }
    }



}
