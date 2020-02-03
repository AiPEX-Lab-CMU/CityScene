using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    Coroutine co;
    int timeInterval = 30;
    int camCount = 1;
    bool playerMode = true;
    // Start is called before the first frame update
    void Start()
    {
        co = StartCoroutine(startController());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator startController()
    {
        
        while(playerMode)
        {
            yield return new WaitForSeconds(timeInterval);
            Camera currentCam = GameObject.Find("Camera" + camCount).GetComponent<Camera>();
            camCount = (camCount + 1) % 11;
            Camera nextCam = GameObject.Find("Camera" + camCount).GetComponent<Camera>();
            //yield return new WaitForSeconds(timeInterval);
            if (playerMode)
            {
                currentCam.enabled = false;
                nextCam.enabled = true;
            }
        }

    }

    public void goToEditMode()
    { 
        playerMode = false;
        GameObject.Find("Camera" + camCount).GetComponent<Camera>().enabled = false;
        GameObject.Find("EditCamera").GetComponent<Camera>().enabled = true;
        GameObject.Find("EditCamera").GetComponent<CameraMovement>().editMode = true;
        StopCoroutine(co);
    }

    public void goToPlayerMode()
    {
        playerMode = true;
        GameObject.Find("EditCamera").GetComponent<Camera>().enabled = false;
        GameObject.Find("Camera" + camCount).GetComponent<Camera>().enabled = true;
        GameObject.Find("EditCamera").GetComponent<CameraMovement>().editMode = false;
        co = StartCoroutine(startController());
    }
}
