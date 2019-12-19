using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPrimitiveAI : MonoBehaviour
{

    WaypointMovement wm;

    // Start is called before the first frame update
    void Start()
    {
        wm = this.gameObject.GetComponent<WaypointMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("r");
        if (other.gameObject.CompareTag("Person"))
        {



            wm.shouldMove = false;

        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Person"))
        {

            wm.shouldMove = true;

        }


    }


}
