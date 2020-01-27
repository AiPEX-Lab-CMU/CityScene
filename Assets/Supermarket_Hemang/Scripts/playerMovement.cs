using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    GameObject trolley;
    // Start is called before the first frame update
    void Start()
    {
        trolley = GameObject.Find("Trolley");
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition.Set(0.0f, 0.0f, -0.1f);
    }
}
