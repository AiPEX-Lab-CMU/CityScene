using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition.Set(0.0f, 0.0f, -0.9f);
    }
}
