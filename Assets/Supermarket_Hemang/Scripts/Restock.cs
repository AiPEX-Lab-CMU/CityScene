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
    private int nextMarkerVal = 1;
    private Camera cam;
    public float moveSpeed = 1.0f;
    SendMessage messageSender;
    bool isMoving = false;
    bool pictured = false;
    int count = 0;
    int checkedOutCustomers = 0;
    float timeLimit = 4000.0f;
    List<string> itemsNeeded = new List<string>();
    public Dictionary<string, HashSet<string>> shelfRestockMap = new Dictionary<string, HashSet<string>>();
    public bool doingCoroutine = false;
    int numCoroutines = 0;



    public IEnumerator communicateWithPython()
    {
        doingCoroutine = true;
        string reply = "";
        float timePassed = 0.0f;
        var stopWatch = new System.Diagnostics.Stopwatch();
        while (timePassed < timeLimit)
        {
            stopWatch.Restart();
            reply = sendCurrentState();
            actBasedOnReply(reply);
            yield return new WaitForSeconds(0.5f);
            stopWatch.Stop();
            timePassed += stopWatch.ElapsedMilliseconds;
        }
        doingCoroutine = false;
        numCoroutines++;
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
    }

    public void customerCheckout()
    {
        checkedOutCustomers++;
        if(checkedOutCustomers != 0 && checkedOutCustomers % 50 == 0)
            readyToMove();
    }

    public void resetPosition()
    {
        Debug.Log("Memory used before collection:       "+
                          GC.GetTotalMemory(false));
        GC.Collect();
        Debug.Log("Memory used after full collection:   "+
                          GC.GetTotalMemory(true));
        gameObject.transform.position = new Vector3(-70f, 0.59f, 1.07f);
        isMoving = false;
        nextMarkerVal = 1;
        numCoroutines = 0;
    }


    public void setIsMoving(bool moving)
    {
        Character character = gameObject.GetComponent<Character>();
        character.characterLocomotion.runSpeed = 0.0f;
        communicateWithPython();
        character.characterLocomotion.runSpeed = moveSpeed;
        isMoving = moving;
        if (nextMarkerVal > 39)
        {
            gameObject.transform.position = new Vector3(-70f, 0.59f, 1.07f);
            return;
        }
    }

    public void readyToMove()
    {
        gameObject.transform.position = new Vector3(-32.616f, 0.265f, -7.769f);
        GameObject shelves = GameObject.Find("shelves");
        shelfRestockMap = new Dictionary<string, HashSet<string>>();
        foreach (Transform shelf in shelves.GetComponentsInChildren<Transform>())
        {
            if (shelf.gameObject.name.ToLower().Contains("ray_store"))
            {
                HashSet<string> listOfItems = new HashSet<string>();
                foreach (Transform child in shelf.gameObject.GetComponentsInChildren<Transform>())
                {
                    string name = child.gameObject.name;
                    if (name.ToLower().Contains("product") && !name.Contains("LOD"))
                    {
                        string tmp = name.Substring(name.IndexOf('_') + 1);
                        int index = tmp.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' });
                        string product;
                        if (index != -1)
                            product = tmp.Substring(0, index);
                        else
                            product = tmp;
                        if (itemsNeeded.Contains(product))
                            listOfItems.Add(product);
                    }
                }
                shelfRestockMap.Add(shelf.gameObject.name, listOfItems);
            }
        }
        numCoroutines = 0;
        isMoving = true;
    }

    public void resetCam()
    {
        cam.transform.rotation = new Quaternion(10.0f, 150.0f, 0.0f, 0.0f);
        cam.fieldOfView = 60.0f;
    }

    // This function sends the current position, orientation, zoom and pixel data
    // of the robot to python.
    // Output format - (position),rotation,zoom,half_empty
    // zoom here is considered to be equivalent to field of view parameter of Unity camera
    private string sendCurrentState()
    {
        Transform t = gameObject.transform;
        string position = "(" + t.position.x + "," + t.position.y + "," + t.position.z + ")";
        string orientation = t.localEulerAngles.y.ToString();
        string zoom = cam.fieldOfView.ToString();
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
        int halfEmpty = 0;
        // send snapshot to python
        RaycastHit hit;
        MarketItems market = GameObject.Find("Green_Market").GetComponent<MarketItems>();
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5.0f))
        {
            string name = hit.collider.gameObject.name;
            if (shelfRestockMap.ContainsKey(name) && shelfRestockMap[name].Count > 0)
            {
                HashSet<string> itemsOnShelf = shelfRestockMap[name];
                int total = 0;
                int current = 0;
                foreach(string product in itemsOnShelf)
                {
                    current += market.currentInventory[product];
                    total += market.initialInventory[product];
                }
                if((float)current/(float)total < 0.5)
                {
                    halfEmpty = 1;
                }
            }
        }
        string reply = messageSender.sendBytesHybrid("002", position + "|" + orientation + "|" + zoom + "|" + halfEmpty + "|", bytes);
        return reply;
    }


    //Parse the reply from python and change values accordingly
    // format of communication from python - rotation|zoom|restock
    private void actBasedOnReply(string reply)
    {
        string[] str = reply.Split('|');
        if (str[2].Equals("1"))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5.0f))
            {
                Debug.Log("need to restock shelf " + hit.collider.gameObject.name);
                MarketItems market = GameObject.Find("Green_Market").GetComponent<MarketItems>();
                string shelfName = hit.collider.gameObject.name;
                if (shelfRestockMap.ContainsKey(shelfName) && shelfRestockMap[shelfName].Count > 0)
                {
                    HashSet<string> itemsToStock = shelfRestockMap[shelfName];
                    foreach (string itemToStock in itemsToStock)
                    {
                        market.restock(itemToStock);
                    }
                }
            }
        }
        Transform t = gameObject.transform;
        float rotAngle = float.Parse(str[0]);
        t.Rotate(new Vector3(0f, rotAngle, 0f));
        float zoom = float.Parse(str[1]);
        cam.fieldOfView = zoom;
    }




    //Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            if (nextMarkerVal > 39)
                return;
            Vector3 target = GameObject.Find("Marker" + nextMarkerVal).transform.position;
            if (Vector3.Distance(gameObject.transform.position, target) < 1.0f)
            {
                if (!doingCoroutine && numCoroutines != nextMarkerVal)
                {
                    StartCoroutine(communicateWithPython());
                }
                if (!doingCoroutine && numCoroutines == nextMarkerVal)
                {
                    nextMarkerVal += 1;
                }
                if (nextMarkerVal > 39)
                {
                    gameObject.transform.position = new Vector3(-70f, 0.59f, 1.07f);
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            }
        }
    }
}
