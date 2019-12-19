using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeModelPosAndRot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {

        this.transform.localPosition = Vector3.zero;

        this.transform.localEulerAngles = Vector3.zero;

    }

}
