using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenshotScript : MonoBehaviour
{
    public int resWidth;
    public int resHeight;
    int count = 0;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        resWidth = 1280;
        resHeight = 720;
        cam = GameObject.Find("screenshotCam").GetComponent<Camera>();
    }


    public void takeScreenShot()
    {
        cam.enabled = true;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        cam.targetTexture = rt;
        Texture2D screen = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;
        screen.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = screen.EncodeToJPG();
        string filename = string.Format("{0}/screen" + count +".jpg", Application.dataPath);
        //call sendToPython function
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log("screenshot taken");
        cam.enabled = false;
        count++;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
