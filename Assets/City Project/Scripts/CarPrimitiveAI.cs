using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPrimitiveAI : MonoBehaviour
{

    NavMeshTry wm;

    int numberOfPeople;

    // Start is called before the first frame update
    void Start()
    {
        numberOfPeople = 0;
        wm = this.gameObject.GetComponent<NavMeshTry>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        /* Register pedestrian encountered */
        if (other.gameObject.CompareTag("Person"))
        {
            numberOfPeople++;
            wm.CarStop();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        /* Deregister pedestrian */
        if (other.gameObject.CompareTag("Person"))
        {
            numberOfPeople--;
            if(numberOfPeople == 0)
                wm.CarGo();
        }


    }


}
