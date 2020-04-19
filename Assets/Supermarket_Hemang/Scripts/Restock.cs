using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using GameCreator.Core;
using GameCreator.Playables;
using GameCreator.Characters;
using GameCreator.Variables;

public class Restock : MonoBehaviour
{
    private int nextMarkerVal = 0;
    private Camera cam;
    public float moveSpeed = 0.25f;
    public float waitSeconds = 0.25f;
    public Vector3 moveDirection = new Vector3(0f, 0f, 0f);
    SendMessage messageSender;
    bool isMoving = false;
    bool pictured = false;
    int count = 0;
    int checkedOutCustomers = 0;
    List<string> itemsNeeded = new List<string>();

    public IEnumerator communicateWithPython()
    {
        string reply = "";
        yield return new WaitForSeconds(1.0f);
        while (!isMoving) ;
        Debug.Log("Start working");
        while(reply != "Stop Moving")
        {
            isMoving = false;
            reply = sendCurrentState();
            actBasedOnReply(reply);
            isMoving = true;
            yield return new WaitForSeconds(waitSeconds);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.transform.GetChild(0).GetComponent<Camera>();
        cam.enabled = false;
        messageSender = (SendMessage)GameObject.Find("messageSender").GetComponent<SendMessage>();
        itemsNeeded.Add("orange");
        itemsNeeded.Add("potatoes");
        itemsNeeded.Add("yogurt");
        itemsNeeded.Add("Cheese");
        itemsNeeded.Add("butter");
        itemsNeeded.Add("beef");
        itemsNeeded.Add("chicken");
        itemsNeeded.Add("onion");
        itemsNeeded.Add("garlic");
        itemsNeeded.Add("pepper_green");
        itemsNeeded.Add("papertowels");
        itemsNeeded.Add("bread_");
        itemsNeeded.Add("milk");
        itemsNeeded.Add("cola");
        itemsNeeded.Add("beer");
        itemsNeeded.Add("wvine");
        Debug.Log("Initial rotation around y axis is " + gameObject.transform.localEulerAngles.y);
    }

    void moveToNextMarker()
    {
        isMoving = true;
        Actions actions = gameObject.transform.GetComponent<Actions>();
        ActionCharacterMoveTo charMove = actions.actionsList.actions[0].GetComponent<ActionCharacterMoveTo>();
        nextMarkerVal++;
        if (nextMarkerVal > 39)
            return;
        charMove.marker = GameObject.Find("Marker" + nextMarkerVal).GetComponent<NavigationMarker>();
        actions.Execute();
    }

    public void customerCheckout()
    {
        checkedOutCustomers++;
        if(checkedOutCustomers == 50)
            readyToMove();
    }

    public void setIsMoving(bool moving)
    {
        Debug.Log("Called is moving");
        isMoving = moving;
    }

    /* Zooms in or out at a random coefficient and focuses on the object and takes a snapshot */
    private void takeSnapshot(Vector3 product)
    {
        /* Focus on the product */
        cam.transform.LookAt(product);
        cam.enabled = true;
        float zoomDistance = 0f;
        cam.fieldOfView += zoomDistance;
        int resWidth = 1280;
        int resHeight = 720;
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
        messageSender.sendBytesInMemory("000", bytes);
        cam.fieldOfView -= zoomDistance;
        count++;
        cam.enabled = false;
    }

    public void readyToMove()
    {
        gameObject.transform.position = new Vector3(-32.616f, 0.265f, -7.769f);
        isMoving = true;
        StartCoroutine(communicateWithPython());
    }

    public void resetCam()
    {
        cam.transform.rotation = new Quaternion(10.0f, 150.0f, 0.0f, 0.0f);
        cam.fieldOfView = 60.0f;
    }

    // This function sends the current position, orientation, zoom and pixel data
    // of the robot to python.
    // Output format - (position),rotation,zoom
    // zoom here is considered to be equivalent to field of view parameter of Unity camera
    private string sendCurrentState()
    {
        Transform t = gameObject.transform;
        string position = "(" + t.position.x + "," + t.position.y + "," + t.position.z + ")";
        string orientation = t.localEulerAngles.y.ToString();
        string zoom = cam.fieldOfView.ToString();
        Debug.Log("Current state : " + position + "|" + orientation + "|" + zoom);
        cam.enabled = true;
        int resWidth = 1280;
        int resHeight = 720;
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
        cam.enabled = false;
        // send snapshot to python
        string reply = messageSender.sendBytesHybrid("002", position + "|" + orientation + "|" + zoom + "|", bytes);
        return reply;
    }


    //Parse the reply from python and change values accordingly
    // format of communication from python - move|rotation|zoom|restock
    private void actBasedOnReply(string reply)
    {
        if (reply == "Stop Moving")
        {
            isMoving = false;
            return;
        }
        string[] str = reply.Split('|');
        if (str[3].Equals("1"))
        {
            Debug.Log("need to restock");
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10.0f))
            {
                Debug.Log("Detected " + hit.collider.gameObject.name);
                MarketItems market = GameObject.Find("Green_Market").GetComponent<MarketItems>();
                GameObject shelf = hit.collider.gameObject;
                HashSet<string> itemsToStock = new HashSet<string>();
                foreach(Transform child in shelf.GetComponentsInChildren<Transform>())
                {
                    string name = child.gameObject.name;
                    if(name.ToLower().Contains("product") && !name.Contains("LOD"))
                    {
                        string tmp = name.Substring(name.IndexOf('_') + 1);
                        int index = tmp.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' });
                        string product;
                        if (index != -1)
                            product = tmp.Substring(0, index);
                        else
                            product = tmp;
                        if(itemsNeeded.Contains(product))
                            itemsToStock.Add(product);
                    }
                }
                foreach(string itemToStock in itemsToStock)
                {
                    market.restock(itemToStock);
                }
            }
        }
        Transform t = gameObject.transform;
        float rotAngle = float.Parse(str[1]);
        t.Rotate(new Vector3(0f, rotAngle, 0f));
        float direction = float.Parse(str[0]);
        if (direction == 0)
            moveDirection = Vector3.forward;
        else if (direction == 3)
            moveDirection = Vector3.back;
        else if (direction == 1)
            moveDirection = Vector3.right;
        else if (direction == 2)
            moveDirection = Vector3.left;
        float zoom = float.Parse(str[2]);
        cam.fieldOfView = zoom;
    }




    //Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            Debug.Log("move direction is " + moveDirection);
            gameObject.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}
