using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MasterDebugLineDraw : MonoBehaviour
{

    DebugLineDraw[] debugLineDraws;

    [SerializeField] bool debugLineDraw;
    bool debugLineDrawInLastFrame;

    // Start is called before the first frame update
    void Start()
    {

        debugLineDraws = GameObject.FindObjectsOfType<DebugLineDraw>();

    }

    // Update is called once per frame
    void Update()
    {
        
        if(debugLineDraw && !debugLineDrawInLastFrame)
        {
            //switched on

            foreach(DebugLineDraw d in debugLineDraws)
            {

                d.drawPathLine = true;

            }

        }

        if(!debugLineDraw && debugLineDrawInLastFrame)
        {

            //switched off

            foreach (DebugLineDraw d in debugLineDraws)
            {

                d.drawPathLine = false;

            }


        }

        debugLineDrawInLastFrame = debugLineDraw;

    }
}
