using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    //The parent gameobject containing multiple particle effect system to create rain
    [SerializeField] GameObject rainParent;

    [SerializeField] Material rainSky;
    [SerializeField] Material startSky;

    bool shouldRain = false;

    // Start is called before the first frame update
    void Start()
    {

        startSky = RenderSettings.skybox;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {

            RenderSettings.skybox = rainSky;

            Invoke("StartRain", 5.0f);

            shouldRain = true;

        }


        if (Input.GetKeyDown(KeyCode.X))
        {

            RenderSettings.skybox = startSky;

            shouldRain = false;

            StopRain();

        }

    }

    void StartRain()
    {

        if (!shouldRain)
            return;

        rainParent.SetActive(true);

    }

    void StopRain()
    {

        rainParent.SetActive(false);

    }

}
